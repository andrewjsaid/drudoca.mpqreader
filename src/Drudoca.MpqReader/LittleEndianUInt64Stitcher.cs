namespace Drudoca.MpqReader
{
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
