namespace Drudoca.MpqReader
{
    internal class MpqFileHeader
    {

        public MpqFileHeader(
            int signature,
            int headerSize,
            int archiveSize,
            ushort formatVersion,
            ushort blockSize,
            int hashTableOffset,
            int blockTableOffset,
            int hashTableCount,
            int blockTableCount)
        {
            Signature = signature;
            HeaderSize = headerSize;
            ArchiveSize = archiveSize;
            FormatVersion = formatVersion;
            BlockSize = blockSize;
            HashTableOffset = hashTableOffset;
            BlockTableOffset = blockTableOffset;
            HashTableCount = hashTableCount;
            BlockTableCount = blockTableCount;
        }

        /// <summary>
        /// The ID_MPQ ('MPQ\x1A') signature
        /// </summary>
        public int Signature { get; }

        /// <summary>
        /// Size of the archive header
        /// </summary>
        public int HeaderSize { get; }

        /// <summary>
        /// Size of MPQ archive
        /// This field is deprecated in the Burning Crusade MoPaQ format, and the size of the archive
        /// is calculated as the size from the beginning of the archive to the end of the hash table,
        /// block table, or extended block table (whichever is largest).
        /// </summary>
        public int ArchiveSize { get; }

        /// <summary>
        /// 0 = Format 1 (up to The Burning Crusade)
        /// 1 = Format 2 (The Burning Crusade and newer)
        /// 2 = Format 3 (WoW - Cataclysm beta or newer)
        /// 3 = Format 4 (WoW - Cataclysm beta or newer)
        /// </summary>
        public ushort FormatVersion { get; }

        /// <summary>
        /// Power of two exponent specifying the number of 512-byte disk sectors in each logical sector
        /// in the archive. The size of each logical sector in the archive is 512 * 2^wBlockSize.
        /// </summary>
        public ushort BlockSize { get; }

        /// <summary>
        /// Offset to the beginning of the hash table, relative to the beginning of the archive.
        /// </summary>
        public int HashTableOffset { get; }

        /// <summary>
        /// Offset to the beginning of the block table, relative to the beginning of the archive.
        /// </summary>
        public int BlockTableOffset { get; }

        /// <summary>
        /// Number of entries in the hash table. Must be a power of two, and must be less than 2^16 for
        /// the original MoPaQ format, or less than 2^20 for the Burning Crusade format.
        /// </summary>
        public int HashTableCount { get; }

        /// <summary>
        /// Number of entries in the block table
        /// </summary>
        public int BlockTableCount { get; }

    }
}
