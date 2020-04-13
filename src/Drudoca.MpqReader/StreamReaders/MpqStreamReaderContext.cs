using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqStreamReaderContext : IDisposable
    {
        private readonly Stream _stream;

        public byte[] Buffer { get; private set; } = Array.Empty<byte>();
        public ByteArrayReader Reader { get; }
        public int BufferSize { get; private set; }

        public MpqStreamReaderContext(Stream stream)
        {
            _stream = stream;
            Reader = new ByteArrayReader(Buffer, 0);
        }

        public async Task ReadAsync(int length)
        {
            if (Buffer.Length < BufferSize + length)
            {
                // Buffer is too small - get another one
                var newBuffer = ArrayPool<byte>.Shared.Rent(BufferSize + length);
                if (Buffer.Length > 0)
                {
                    Array.Copy(Buffer, newBuffer, BufferSize);
                    ArrayPool<byte>.Shared.Return(Buffer);
                }
                Buffer = newBuffer;
                Reader.SetData(Buffer);
            }

            var readResult = await _stream.ReadAsync(Buffer, BufferSize, length);
            if (readResult != length)
            {
                throw new InvalidDataException($"Could not read enough bytes.");
            }

            BufferSize += length;
        }

        public void Dispose()
        {
            ArrayPool<byte>.Shared.Return(Buffer);
        }
    }
}
