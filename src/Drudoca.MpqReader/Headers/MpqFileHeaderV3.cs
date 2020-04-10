namespace Drudoca.MpqReader
{
    internal class MpqFileHeaderV3 : MpqFileHeaderV2
    {

        public MpqFileHeaderV3(
            int signature,
            int headerSize,
            int archiveSize,
            ushort formatVersion,
            ushort blockSize,
            int hashTableOffset,
            int blockTableOffset,
            int hashTableCount,
            int blockTableCount,
            ulong hiBlockTableOffset,
            ushort hashTableOffsetHi,
            ushort blockTableOffsetHi,
            ulong archiveSize2,
            ulong betTableOffset,
            ulong hetTableOffset)
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
        public ulong ArchiveSize2 { get; }

        /// <summary>
        /// 64-bit position of the BET table
        /// </summary>
        public ulong BetTableOffset { get; }

        /// <summary>
        /// 64-bit position of the HET table
        /// </summary>
        public ulong HetTableOffset { get; }
    }
}
