using System;

namespace Drudoca.MpqReader
{
    internal sealed class ByteArrayReader
    {
        private readonly byte[] _buffer;
        private int _index;

        public ByteArrayReader(byte[] buffer, int index)
        {
            _buffer = buffer;
            _index = index;
        }

        public byte ReadByte() => _buffer[_index++];

        public int ReadInt32()
        {
            var result = BitConverter.ToInt32(_buffer[_index..(_index + 4)]); ;
            _index += 4;
            if (!BitConverter.IsLittleEndian)
            {
                result = SwapEndianness(result);
            }
            return result;
        }

        public ulong ReadUInt64()
        {
            var result = BitConverter.ToUInt64(_buffer[_index..(_index + 8)]); ;
            _index += 8;
            if (!BitConverter.IsLittleEndian)
            {
                result = SwapEndianness(result);
            }
            return result;
        }

        public ushort ReadUInt16()
        {
            var result = BitConverter.ToUInt16(_buffer[_index..(_index + 2)]); ;
            _index += 2;
            if (!BitConverter.IsLittleEndian)
            {
                result = SwapEndianness(result);
            }
            return result;
        }

        public byte[] ReadByteArray(int length)
        {
            var result = new byte[length];
            Array.Copy(_buffer, _index, result, 0, length);
            _index += length;
            return result;
        }

        private static int SwapEndianness(int value)
        {
            unchecked
            {
                var b1 = (value >> 0) & 0xff;
                var b2 = (value >> 8) & 0xff;
                var b3 = (value >> 16) & 0xff;
                var b4 = (value >> 24) & 0xff;

                return b1 << 24 | b2 << 16 | b3 << 8 | b4 << 0;
            }
        }

        private static ushort SwapEndianness(ushort value)
        {
            unchecked
            {
                var intValue = (uint)value;
                var b1 = (intValue >> 0) & (uint)0xff;
                var b2 = (intValue >> 8) & (uint)0xff;

                return (ushort)(b1 << 8 | b2 << 0);
            }
        }

        private static ulong SwapEndianness(ulong value)
        {
            unchecked
            {
                var b1 = (value >> 0) & 0xfful;
                var b2 = (value >> 8) & 0xfful;
                var b3 = (value >> 8) & 0xfful;
                var b4 = (value >> 8) & 0xfful;
                var b5 = (value >> 8) & 0xfful;
                var b6 = (value >> 8) & 0xfful;
                var b7 = (value >> 8) & 0xfful;
                var b8 = (value >> 8) & 0xfful;

                return b1 << 56 | b2 << 48 | b3 << 40 | b4 << 32 |
                       b5 << 24 | b6 << 16 | b7 << 8 | b8 << 0;
            }
        }
    }
}
