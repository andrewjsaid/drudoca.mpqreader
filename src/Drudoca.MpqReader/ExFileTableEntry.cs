namespace Drudoca.MpqReader
{
    internal class ExFileTableEntry : IFileTableEntry
    {

        public ExFileTableEntry(
            byte hetHash,
            ulong betHash,
            long fileOffset,
            long compressedFileSize,
            long fileSize,
            BlockFileFlags flags)
        {
            HetHash = hetHash;
            BetHash = betHash;
            FileOffset = fileOffset;
            CompressedFileSize = compressedFileSize;
            FileSize = fileSize;
            Flags = flags;
        }

        public byte HetHash { get; }

        public ulong BetHash { get; }

        public long FileOffset { get; }

        public long CompressedFileSize { get; }

        public long FileSize { get; }

        public BlockFileFlags Flags { get; }

    }
}
