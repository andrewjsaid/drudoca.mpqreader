namespace Drudoca.MpqReader.Structures
{
    internal class MpqArchiveHeaderV3 : MpqArchiveHeaderV2
    {

        public MpqArchiveHeaderV3(
            int signature,
            int headerSize,
            int archiveSize,
            ushort formatVersion,
            ushort blockSize,
            int hashTableOffset,
            int blockTableOffset,
            int hashTableCount,
            int blockTableCount,
            long hiBlockTableOffset,
            ushort hashTableOffsetHi,
            ushort blockTableOffsetHi,
            long archiveSize2,
            long betTableOffset,
            long hetTableOffset)
            : base(signature, headerSize, archiveSize,
                   formatVersion, blockSize,
                   hashTableOffset, blockTableOffset,
                   hashTableCount, blockTableCount,
                   hiBlockTableOffset, hashTableOffsetHi, blockTableOffsetHi)
        {
            ArchiveSize2 = archiveSize2;
            BetTableOffset = betTableOffset;
            HetTableOffset = hetTableOffset;
        }

        /// <summary>
        /// 64-bit version of the archive size
        /// </summary>
        public long ArchiveSize2 { get; }

        /// <summary>
        /// 64-bit position of the BET table
        /// </summary>
        public long BetTableOffset { get; }

        /// <summary>
        /// 64-bit position of the HET table
        /// </summary>
        public long HetTableOffset { get; }
    }
}
