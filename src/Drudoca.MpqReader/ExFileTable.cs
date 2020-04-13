namespace Drudoca.MpqReader
{
    internal class ExFileTable
    {
        public ExFileTable(ExFileTableEntry?[] entries)
        {
            Entries = entries;
        }

        public ExFileTableEntry?[] Entries { get; }
    }
}
