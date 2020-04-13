namespace Drudoca.MpqReader.Structures
{
    internal class MpqArchiveHeaderV2 : MpqArchiveHeader
    {

        public MpqArchiveHeaderV2(
            int archiveSize,
            ushort formatVersion,
            ushort blockSize,
            int hashTableOffset,
            int blockTableOffset,
            int hashTableCount,
            int blockTableCount,
            long hiBlockTableOffset,
            ushort hashTableOffsetHi,
            ushort blockTableOffsetHi)
            : base(archiveSize,
                   formatVersion, blockSize,
                   hashTableOffset, blockTableOffset,
                   hashTableCount, blockTableCount)
        {
            HiBlockTableOffset = hiBlockTableOffset;
            HashTableOffsetHi = hashTableOffsetHi;
            BlockTableOffsetHi = blockTableOffsetHi;
        }

        /// <summary>
        /// Offset to the beginning of array of 16-bit high parts of file offsets.
        /// </summary>
        public long HiBlockTableOffset { get; }

        /// <summary>
        /// High 16 bits of the hash table offset for large archives.
        /// </summary>
        public ushort HashTableOffsetHi { get; }

        /// <summary>
        /// High 16 bits of the block table offset for large archives.
        /// </summary>
        public ushort BlockTableOffsetHi { get; }

    }
}
