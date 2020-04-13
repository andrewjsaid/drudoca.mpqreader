using System;

namespace Drudoca.MpqReader
{
    internal class ByteArrayReader
    {

        private byte[] _data;
        private int _index;

        public ByteArrayReader(
            byte[] data,
            int startIndex)
        {
            _data = data;
            _index = startIndex;
        }

        internal void SetData(byte[] data) => _data = data;

        public void Advance(int bytes)
        {
            _index += bytes;
        }

        public byte ReadByte() => _data[_index++];

        public ushort ReadUInt16()
        {
            var result = BitConverter.ToUInt16(_data[_index..(_index + 2)]);
            _index += 2;
            return result;
        }

        public int ReadInt16()
        {
            var result = BitConverter.ToInt16(_data[_index..(_index + 2)]);
            _index += 2;
            return result;
        }

        public uint ReadUInt32()
        {
            var result = BitConverter.ToUInt32(_data[_index..(_index + 4)]);
            _index += 4;
            return result;
        }

        public int ReadInt32()
        {
            var result = BitConverter.ToInt32(_data[_index..(_index + 4)]);
            _index += 4;
            return result;
        }

        public ulong ReadUInt64()
        {
            var result = BitConverter.ToUInt64(_data[_index..(_index + 8)]);
            _index += 8;
            return result;
        }

        public long ReadInt64()
        {
            var result = BitConverter.ToInt64(_data[_index..(_index + 8)]);
            _index += 8;
            return result;
        }

        public byte[] ReadByteArray(int length)
        {
            var result = new byte[length];
            Array.Copy(_data, _index, result, 0, length);
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

                if (bitOffset == 0)
                {
                    b = _data[b0];
                }
                else
                {
                    var x0 = _data[b0];
                    var x1 = _data[b1];
                    b = (byte)(((x0 >> bitOffset) | (x1 << (8 - bitOffset))) & 0xFF);
                }

                result.Append(b);
                b0++;
                b1++;
            }

            var remSize = bitSize & 7;
            if (remSize > 0)
            {
                var x0 = _data[b0];
                var x1 = _data[b1];

                var b = (byte)(x0 >> bitOffset);

                if (remSize > 8 - bitOffset)
                {
                    b = (byte)(((x0 >> bitOffset) | (x1 << (8 - bitOffset))) & 0xFF);
                }

                b = (byte)(b & (byte)(0x01 << remSize) - 1);

                result.Append(b);
            }

            return result.Value;
        }
    }
}
