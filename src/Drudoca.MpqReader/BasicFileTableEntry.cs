namespace Drudoca.MpqReader
{
    /// <summary>
    /// Represents an entry in the combined
    /// Hash Table and Block Table.
    /// </summary>
    internal class BasicFileTableEntry : IFileTableEntry
    {

        public BasicFileTableEntry(
            uint nameHash1,
            uint nameHash2,
            ushort locale,
            ushort platform,
            bool isDeleted,
            long fileOffset,
            long compressedFileSize,
            long fileSize,
            BlockFileFlags flags)
        {
            NameHash1 = nameHash1;
            NameHash2 = nameHash2;
            Locale = locale;
            Platform = platform;
            IsDeleted = isDeleted;
            FileOffset = fileOffset;
            CompressedFileSize = compressedFileSize;
            FileSize = fileSize;
            Flags = flags;
        }

        public uint NameHash1 { get; }

        public uint NameHash2 { get; }

        /// <summary>
        /// Windows LANGID data type, and uses the same values.
        /// 0 indicates the default language (American English), or that the file is language-neutral.
        /// </summary>
        public ushort Locale { get; }

        public ushort Platform { get; set; }

        public bool IsDeleted { get; }

        public long FileOffset { get; }

        public long CompressedFileSize { get; }

        public long FileSize { get; }

        public BlockFileFlags Flags { get; }
    }
}
