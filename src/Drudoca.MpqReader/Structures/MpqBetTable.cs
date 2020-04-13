namespace Drudoca.MpqReader.Structures
{
    internal class MpqBetTable
    {

        public MpqBetTable(
            int numEntries,
            MpqBlockTable[] blockTable,
            ulong[] betHashTable)
        {
            NumEntries = numEntries;
            BlockTable = blockTable;
            BetHashTable = betHashTable;
        }

        /// <summary>
        /// Number of files in the BET table
        /// </summary>
        public int NumEntries { get; }

        /// <summary>
        /// Number of flags in the following array
        /// </summary>
        public int FlagCount { get; }

        public MpqBlockTable[] BlockTable { get; }

        public ulong[] BetHashTable { get; }
    }
}
