using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;

namespace Drudoca.MpqReader
{
    internal abstract class MpqStreamReaderBase<T>
    {
        private Stream _stream;
        private ByteArrayReader? _bar;
        private byte[]? _buffer;

        protected byte[] Buffer => _buffer ?? throw new InvalidOperationException("Only able to access buffer within Read context");
        
        protected int BufferSize { get; private set; }

        public MpqStreamReaderBase(Stream stream)
        {
            _stream = stream;
        }

        protected abstract int InitialSize { get; }
        protected abstract ValueTask<T> ReadAsync(ByteArrayReader bar);

        public async Task<T> ReadAsync()
        {
            BufferSize = InitialSize;
            _buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
            try
            {
                var readResult = await _stream.ReadAsync(_buffer, 0, BufferSize);
                if (readResult != BufferSize)
                {
                    throw new InvalidDataException($"Could not read enough bytes.");
                }

                _bar = new ByteArrayReader(_buffer, 0);
                return await ReadAsync(_bar);
            }
            finally
            {
                _bar = null;
                _buffer = null;
                ArrayPool<byte>.Shared.Return(_buffer);
            }
        }

        public async Task<T[]> ReadManyAsync(Stream stream, int count)
        {
            BufferSize = InitialSize * count;
            _buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
            try
            {
                var readResult = await stream.ReadAsync(_buffer, 0, BufferSize);
                if (readResult != BufferSize)
                {
                    throw new InvalidDataException($"Could not read enough bytes.");
                }

                _bar = new ByteArrayReader(_buffer, 0);
                var results = new T[count];
                for (int i = 0; i < count; i++)
                {
                    var r = await ReadAsync(_bar);
                    results[i] = r;
                }
                return results;
            }
            finally
            {
                _bar = null;
                _buffer = null;
                ArrayPool<byte>.Shared.Return(_buffer);
            }
        }

        protected async Task GrowAsync(int length)
        {
            if (_buffer == null || _bar == null)
                throw new InvalidOperationException("Must be within ReadAsync or ReadManyAsync to grow buffer.");

            if (_buffer.Length < BufferSize + length)
            {
                // Buffer is too small - get another one
                var newBuffer = ArrayPool<byte>.Shared.Rent(BufferSize + length);
                Array.Copy(_buffer, newBuffer, length);
                ArrayPool<byte>.Shared.Return(_buffer);
                _buffer = newBuffer;
                _bar.ReplaceBuffer(_buffer);
                BufferSize += length;
            }

            var readResult = await _stream.ReadAsync(_buffer, BufferSize, length);
            if (readResult != length)
            {
                throw new InvalidDataException($"Could not read enough bytes.");
            }
        }

    }
}
