using System;
using System.IO;

namespace Drudoca.MpqReader.Extraction
{
    internal class CustomMemoryStream : Stream
    {
        private readonly Memory<byte> _memory;
        private int _index;

        public CustomMemoryStream(Memory<byte> memory)
        {
            _memory = memory;
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => true;
        public override long Length => _memory.Length;

        public override long Position
        {
            get => _index;
            set
            {
                if (value < 0 || value >= _memory.Length)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _index = (int)value;
            }
        }

        public override void Flush() { }

        public override long Seek(long offset, SeekOrigin origin)
        {
            Position =
                origin switch
                {
                    SeekOrigin.Begin => offset,
                    SeekOrigin.Current => _index + offset,
                    SeekOrigin.End => _memory.Length - offset,
                    _ => throw new InvalidOperationException()
                };
            return Position;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var source = _memory.Slice(_index);
            var length = Math.Min(source.Length, count);
            source.Slice(0, count)
                  .CopyTo(buffer.AsMemory().Slice(offset, count));
            _index += length;
            return length;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var source = _memory.Slice(_index, count);
            if (count > source.Length)
                throw new ArgumentException("Overflow");

            buffer.AsMemory(offset, count)
                  .CopyTo(source);
            _index += count;
        }

        public override void SetLength(long value) => throw new NotSupportedException();
    }
}
