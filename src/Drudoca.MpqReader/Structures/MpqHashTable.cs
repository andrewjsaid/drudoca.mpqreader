namespace Drudoca.MpqReader.Structures
{
    internal class MpqHashTable
    {

        public MpqHashTable(
            int nameHash1,
            int nameHash2,
            ushort locale,
            ushort platform,
            uint blockIndex)
        {
            NameHash1 = nameHash1;
            NameHash2 = nameHash2;
            Locale = locale;
            Platform = platform;
            BlockIndex = blockIndex;
        }

        /// <summary>
        /// Hash of the full file name, part 1.
        /// </summary>
        public int NameHash1 { get; }

        /// <summary>
        /// Hash of the full file name, part 2.
        /// </summary>
        public int NameHash2 { get; }

        /// <summary>
        /// The language of the file. This is a Windows LANGID data type, and uses the same values.
        /// 0 indicates the default language (American English), or that the file is language-neutral.
        /// </summary>
        public ushort Locale { get; }

        /// <summary>
        /// The platform the file is used for. 0 indicates the default platform.
        /// No other values have been observed.
        /// </summary>
        public ushort Platform { get; set; }

        /// <summary>
        /// If the hash table entry is valid, this is the index into the block table of the file.
        /// Otherwise, one of the following two values:
        ///  - FFFFFFFFh: Hash table entry is empty, and has always been empty.
        ///               Terminates searches for a given file.
        ///  - FFFFFFFEh: Hash table entry is empty, but was valid at some point (a deleted file).
        ///               Does not terminate searches for a given file.
        /// </summary>
        public uint BlockIndex { get; }
    }
}
