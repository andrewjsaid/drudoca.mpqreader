namespace Drudoca.MpqReader.Headers
{
    internal class MpqBetTable
    {
        /// <summary>
        /// 'BET\x1A'
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
        /// Number of files in the BET table
        /// </summary>
        public int FileCount { get; }

        /// <summary>
        /// Unknown, set to 0x10
        /// </summary>
        public int Unknown08 { get; }

        /// <summary>
        /// Size of one table entry (in bits)
        /// </summary>
        public int TableEntryBitSize { get; }

        /// <summary>
        /// Bit index of the file position (within the entry record)
        /// </summary>
        public int FilePositionBitIndex { get; }

        /// <summary>
        /// Bit index of the file size (within the entry record)
        /// </summary>
        public int FileSizeBitIndex { get; }

        /// <summary>
        /// Bit index of the compressed size (within the entry record)
        /// </summary>
        public int CompressedSizeBitIndex { get; }

        /// <summary>
        /// Bit index of the flag index (within the entry record)
        /// </summary>
        public int FlagIndexBitIndex { get; }

        /// <summary>
        /// Bit index of the ??? (within the entry record)
        /// </summary>
        public int UnknownBitIndex { get; }

        /// <summary>
        /// Bit size of file position (in the entry record)
        /// </summary>
        public int FilePositionBitSize { get; }

        /// <summary>
        /// Bit size of file size (in the entry record)
        /// </summary>
        public int FileSizeBitSize { get; }

        /// <summary>
        /// Bit size of compressed file size (in the entry record)
        /// </summary>
        public int CompressedSizeBitSize { get; }

        /// <summary>
        /// Bit size of flags index (in the entry record)
        /// </summary>
        public int FlagIndexBitSize { get; }

        /// <summary>
        /// Bit size of ??? (in the entry record)
        /// </summary>
        public int UnknownBitSize { get; }

        /// <summary>
        /// Total size of the BET hash
        /// </summary>
        public int BetHashTotalSize { get; }

        /// <summary>
        /// Extra bits in the BET hash
        /// </summary>
        public int BetHashExtraBitSize { get; }

        /// <summary>
        /// Effective size of BET hash (in bits)
        /// </summary>
        public int BetHashBitSize { get; }

        /// <summary>
        /// Size of BET hashes array, in bytes
        /// </summary>
        public int BetHashArraySize { get; }

        /// <summary>
        /// Number of flags in the following array
        /// </summary>
        public int FlagCount { get; }

        /// <summary>
        /// Array of file flags.
        /// </summary>
        public int[] FlagsArray { get; }

        // TODO AJS: Add the following

        // File table. Size of each entry is taken from dwTableEntrySize.
        // Size of the table is (dwTableEntrySize * dwMaxFileCount), round up to 8.

        // Array of BET hashes. Table size is taken from dwMaxFileCount from HET table
    }
}
