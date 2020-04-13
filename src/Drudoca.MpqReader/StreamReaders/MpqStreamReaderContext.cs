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
        public int BufferSize { get; private set; }

        private int _index;

        public MpqStreamReaderContext(Stream stream)
        {
            _stream = stream;
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
            }

            var readResult = await _stream.ReadAsync(Buffer, BufferSize, length);
            if (readResult != length)
            {
                throw new InvalidDataException($"Could not read enough bytes.");
            }

            BufferSize += length;
        }

        public void Advance(int bytes) => _index += bytes;

        public byte ReadByte() => Buffer[_index++];

        public ushort ReadUInt16()
        {
            var result = BitConverter.ToUInt16(Buffer[_index..(_index + 2)]);
            _index += 2;
            return result;
        }

        public int ReadInt16()
        {
            var result = BitConverter.ToInt16(Buffer[_index..(_index + 2)]);
            _index += 2;
            return result;
        }

        public uint ReadUInt32()
        {
            var result = BitConverter.ToUInt32(Buffer[_index..(_index + 4)]);
            _index += 4;
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

        public ulong ReadBits(int bitIndex, int bitSize)
        {
            // Based on StormLib GetBits

            var result = new LittleEndianUInt64Stitcher();

            var b0 = _index + (bitIndex / 8);
            var b1 = b0 + 1;
            var bitOffset = bitIndex & 7;

            var numWholeBytes = bitSize / 8;
            while (numWholeBytes-- > 0)
            {
                byte b;

                if(bitOffset == 0)
                {
                    b = Buffer[b0];
                }
                else
                {
                    var x0 = Buffer[b0];
                    var x1 = Buffer[b1];
                    b = (byte)(((x0 >> bitOffset) | (x1 << (8 - bitOffset))) & 0xFF);
                }

                result.Append(b);
                b0++;
                b1++;
            }

            var remSize = bitSize & 7;
            if(remSize > 0)
            {
                var x0 = Buffer[b0];
                var x1 = Buffer[b1];

                var b = (byte)(x0 >> bitOffset);

                if(remSize > 8 - bitOffset)
                {
                    b = (byte)(((x0 >> bitOffset) | (x1 << (8 - bitOffset))) & 0xFF);
                }

                b = (byte)(b & (byte)(0x01 << remSize) - 1);

                result.Append(b);
            }

            return result.Value;
        }

        public void Dispose()
        {
            ArrayPool<byte>.Shared.Return(Buffer);
        }
    }

    internal struct LittleEndianUInt64Stitcher
    {
        private int _shift;

        public LittleEndianUInt64Stitcher(ulong value)
        {
            _shift = 0;
            Value = value;
        }

        public ulong Value { get; private set; }

        public void Append(byte b)
        {
            Value |= ((ulong)b << _shift);
            _shift += 8;
        }
    }
}
