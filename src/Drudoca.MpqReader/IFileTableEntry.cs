namespace Drudoca.MpqReader
{
    internal interface IFileTableEntry
    {
        long FileOffset { get; }
        long CompressedFileSize { get; }
        long FileSize { get; }
        BlockFileFlags Flags { get; }
    }
}
