namespace Drudoca.MpqReader.Structures
{
    internal class MpqHetTable
    {

        public MpqHetTable(
            int signature,
            int version,
            int dataSize,
            int tableSize,
            int numEntries,
            int numSlots,
            int hashEntryBitSize,
            int totalIndexBitSize,
            int indexExtraBitSize,
            int indexBitSize,
            int indexTableSize,
            byte[] hashTable,
            long[] fileIndices)
        {
            Signature = signature;
            Version = version;
            DataSize = dataSize;
            TableSize = tableSize;
            NumEntries = numEntries;
            NumSlots = numSlots;
            HashEntryBitSize = hashEntryBitSize;
            TotalIndexBitSize = totalIndexBitSize;
            IndexExtraBitSize = indexExtraBitSize;
            IndexBitSize = indexBitSize;
            IndexTableSize = indexTableSize;
            HashTable = hashTable;
            FileIndices = fileIndices;
        }

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
        /// Size of the entire hash table, including the het header but not the table header.
        /// i.e. should be equal to <see cref="DataSize"/>.
        /// </summary>
        public int TableSize { get; }

        /// <summary>
        /// Number of files in the archive.
        /// </summary>
        public int NumEntries { get; }

        /// <summary>
        /// Max number of entries in the hash table
        /// </summary>
        public int NumSlots { get; }

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
        /// Size of the index subtable (in bytes)
        /// </summary>
        public int IndexTableSize { get; }

        /// <summary>
        /// HET hash table.
        /// </summary>
        public byte[] HashTable { get; }

        /// <summary>
        /// Array of file indices.
        /// </summary>
        public long[] FileIndices { get; }
    }
}
