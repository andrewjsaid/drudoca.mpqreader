namespace Drudoca.MpqReader.Structures
{
    internal class MpqHetTable
    {

        public MpqHetTable(
            int numUsedEntries,
            int numEntries,
            byte[] nameHashes,
            long[] betIndices)
        {
            NumUsedEntries = numUsedEntries;
            NumEntries = numEntries;
            NameHashes = nameHashes;
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
        /// HET name hashes.
        /// </summary>
        public byte[] NameHashes { get; }

        /// <summary>
        /// Array of indices to look up in the BET Table.
        /// </summary>
        public long[] BetIndices { get; }
    }
}
