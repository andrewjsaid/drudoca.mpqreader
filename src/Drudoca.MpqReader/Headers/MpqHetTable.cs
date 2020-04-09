namespace Drudoca.MpqReader.Headers
{
    internal class MpqHetTable
    {
        /// <summary>
        /// 'HET\x1A'
        /// </summary>
        public int Signature { get; }

        /// <summary>
        /// Version. Seems to be always 1
        /// </summary>
        public int Version { get; }

        /// <summary>
        /// Size of the contained table
        /// </summary>
        public int DataSize { get; }

        /// <summary>
        /// Size of the entire hash table, including the header (in bytes)
        /// </summary>
        public int TableSize { get; }

        /// <summary>
        /// Maximum number of files in the MPQ
        /// </summary>
        public int MaxFileCount { get; }

        /// <summary>
        /// Size of the hash table (in bytes)
        /// </summary>
        public int HashTableSize { get; }

        /// <summary>
        /// Effective size of the hash entry (in bits)
        /// </summary>
        public int HashEntryBitSize { get; }

        /// <summary>
        /// Total size of file index (in bits)
        /// </summary>
        public int TotalIndexBitSize { get; }

        /// <summary>
        /// Extra bits in the file index
        /// </summary>
        public int IndexExtraBitSize { get; }

        /// <summary>
        /// Effective size of the file index (in bits)
        /// </summary>
        public int IndexBitSize { get; }

        /// <summary>
        /// Size of the block index subtable (in bytes)
        /// </summary>
        public int BlockTableSize { get; }

        /// <summary>
        /// HET hash table. Each entry is 8 bits.
        /// Thereare <see cref="HashTableSize"/> entries.
        /// </summary>
        public byte[] HetHashTable { get; }

        /// <summary>
        /// Array of file indices.
        /// </summary>
        public long[] FileIndices { get; }
    }
}
