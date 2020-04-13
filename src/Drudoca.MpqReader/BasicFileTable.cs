namespace Drudoca.MpqReader
{
    internal class BasicFileTable
    {
        public BasicFileTable(BasicFileTableEntry?[] entries)
        {
            Entries = entries;
        }

        public int Length => Entries?.Length ?? default;
        public BasicFileTableEntry?[] Entries { get; }
    }
}
