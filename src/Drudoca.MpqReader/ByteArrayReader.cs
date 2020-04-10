using System;

namespace Drudoca.MpqReader
{
    internal sealed class ByteArrayReader
    {
        private byte[] _buffer;
        private int _index;

        public ByteArrayReader(byte[] buffer, int index)
        {
            _buffer = buffer;
            _index = index;
        }

        public void ReplaceBuffer(byte[] buffer) => _buffer = buffer;

        public byte ReadByte() => _buffer[_index++];

        public int ReadInt32()
        {
            var result = BitConverter.ToInt32(_buffer[_index..(_index + 4)]); ;
            _index += 4;
            return result;
        }

        public ulong ReadUInt64()
        {
            var result = BitConverter.ToUInt64(_buffer[_index..(_index + 8)]); ;
            _index += 8;
            return result;
        }

        public ushort ReadUInt16()
        {
            var result = BitConverter.ToUInt16(_buffer[_index..(_index + 2)]); ;
            _index += 2;
            return result;
        }

        public byte[] ReadByteArray(int length)
        {
            var result = new byte[length];
            Array.Copy(_buffer, _index, result, 0, length);
            _index += length;
            return result;
        }
    }
}
