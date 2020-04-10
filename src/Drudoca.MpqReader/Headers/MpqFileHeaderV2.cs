namespace Drudoca.MpqReader.Headers
{
    internal class MpqFileHeaderV2 : MpqFileHeader
    {

        public MpqFileHeaderV2(
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
            ushort blockTableOffsetHi)
            : base(signature, headerSize, archiveSize,
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
        public ulong HiBlockTableOffset { get; }

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
