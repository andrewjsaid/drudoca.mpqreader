namespace Drudoca.MpqReader.Structures
{
    public class MpqBlockTable
    {

        public MpqBlockTable(
            int fileOffset,
            int compressedFileSize,
            int fileSize,
            int flags)
        {
            FileOffset = fileOffset;
            CompressedFileSize = compressedFileSize;
            FileSize = fileSize;
            Flags = flags;
        }

        /// <summary>
        /// Offset of the beginning of the file data, relative to the beginning of the archive.
        /// </summary>
        public int FileOffset { get; }

        /// <summary>
        /// Compressed File Size
        /// </summary>
        public int CompressedFileSize { get; }

        /// <summary>
        /// File Size
        /// </summary>
        public int FileSize { get; }

        /// <summary>
        /// Flags for the file.
        /// </summary>
        public int Flags { get; }

    }
}
