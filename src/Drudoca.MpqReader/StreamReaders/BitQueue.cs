using System.Diagnostics;

namespace Drudoca.MpqReader.StreamReaders
{
    internal struct BitQueue
    {
        public BitQueue(long data, int length)
        {
            Data = data;
            Length = length;
        }

        public long Data { get; private set; }
        public int Length { get; private set; }

        public long TakeAll()
        {
            var result = Data;
            Data = 0;
            Length = 0;
            return result;
        }

        public void Append(byte data)
        {
            Data = (Data << 8) | data;
            Length += 8;
            Debug.Assert(Length <= 64);
        }

        public long Take(int numBits)
        {
            Debug.Assert(numBits <= Length);
            var shift = Length - numBits;
            var result = Data >> shift;
            Data &= ~(~0L << shift);
            Length = shift;
            return result;
        }
    }
}
