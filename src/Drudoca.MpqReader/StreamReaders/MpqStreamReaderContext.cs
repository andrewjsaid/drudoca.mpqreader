﻿using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqStreamReaderContext : IDisposable
    {
        private Stream _stream;

        public byte[] Buffer { get; private set; }
        public int BufferSize { get; private set; }

        private int _index;

        public MpqStreamReaderContext(Stream stream, int initialBufferSize)
        {
            _stream = stream;
            Buffer = ArrayPool<byte>.Shared.Rent(initialBufferSize);
            BufferSize = initialBufferSize;
        }

        public async Task SetupAsync()
        {
            var readResult = await _stream.ReadAsync(Buffer, 0, BufferSize);
            if (readResult != BufferSize)
            {
                throw new InvalidDataException($"Could not read enough bytes.");
            }
        }

        public async Task GrowAsync(int length)
        {
            if (Buffer.Length < BufferSize + length)
            {
                // Buffer is too small - get another one
                var newBuffer = ArrayPool<byte>.Shared.Rent(BufferSize + length);
                Array.Copy(Buffer, newBuffer, BufferSize);
                ArrayPool<byte>.Shared.Return(Buffer);
                Buffer = newBuffer;
            }

            var readResult = await _stream.ReadAsync(Buffer, BufferSize, length);
            if (readResult != length)
            {
                throw new InvalidDataException($"Could not read enough bytes.");
            }

            BufferSize += length;
        }

        public byte ReadByte() => Buffer[_index++];

        public ushort ReadUInt16()
        {
            var result = BitConverter.ToUInt16(Buffer[_index..(_index + 2)]);
            _index += 2;
            return result;
        }

        public int ReadInt32()
        {
            var result = BitConverter.ToInt32(Buffer[_index..(_index + 4)]);
            _index += 4;
            return result;
        }

        public ulong ReadUInt64()
        {
            var result = BitConverter.ToUInt64(Buffer[_index..(_index + 8)]);
            _index += 8;
            return result;
        }

        public long ReadInt64()
        {
            var result = BitConverter.ToInt64(Buffer[_index..(_index + 8)]);
            _index += 8;
            return result;
        }

        public byte[] ReadByteArray(int length)
        {
            var result = new byte[length];
            Array.Copy(Buffer, _index, result, 0, length);
            _index += length;
            return result;
        }

        public void Dispose()
        {
            ArrayPool<byte>.Shared.Return(Buffer);
        }
    }
}
