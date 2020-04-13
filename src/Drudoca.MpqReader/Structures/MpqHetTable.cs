namespace Drudoca.MpqReader.Structures
{
    internal class MpqHetTable
    {

        public MpqHetTable(
            int numUsedEntries,
            int numEntries,
            byte[] hetHashTable,
            long[] betIndices)
        {
            NumUsedEntries = numUsedEntries;
            NumEntries = numEntries;
            HetHashTable = hetHashTable;
            BetIndices = betIndices;
        }

        /// <summary>
        /// Number of files in the archive.
        /// </summary>
        public int NumUsedEntries { get; }

        /// <summary>
        /// Max number of entries in the hash table
        /// </summary>
        public int NumEntries { get; }

        /// <summary>
        /// HET hash table.
        /// </summary>
        public byte[] HetHashTable { get; }

        /// <summary>
        /// Array of indices to look up in the BET Table.
        /// </summary>
        public long[] BetIndices { get; }
    }
}
