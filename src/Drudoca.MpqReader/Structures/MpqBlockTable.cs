namespace Drudoca.MpqReader.Structures
{
    internal class MpqBlockTable
    {

        public MpqBlockTable(
            long fileOffset,
            long compressedFileSize,
            long fileSize,
            uint flags)
        {
            FileOffset = fileOffset;
            CompressedFileSize = compressedFileSize;
            FileSize = fileSize;
            Flags = flags;
        }

        /// <summary>
        /// Offset of the beginning of the file data, relative to the beginning of the archive.
        /// </summary>
        public long FileOffset { get; }

        /// <summary>
        /// Compressed File Size (as found in archive).
        /// </summary>
        public long CompressedFileSize { get; }

        /// <summary>
        /// File Size
        /// </summary>
        public long FileSize { get; }

        /// <summary>
        /// Flags for the file.
        /// </summary>
        public uint Flags { get; }

    }
}
