namespace Drudoca.MpqReader.Structures
{
    internal class MpqUserDataHeader
    {
        public MpqUserDataHeader(
            int userDataSize,
            int headerOffset,
            int userDataHeaderSize)
        {
            UserDataSize = userDataSize;
            HeaderOffset = headerOffset;
            UserDataHeaderSize = userDataHeaderSize;
        }

        /// <summary>
        /// Maximum size of the user data
        /// </summary>
        public int UserDataSize { get; }

        /// <summary>
        /// Offset of the MPQ header, relative to the begin of this header
        /// </summary>
        public int HeaderOffset { get; }

        /// <summary>
        /// Appears to be size of user data header (Starcraft II maps)
        /// </summary>
        public int UserDataHeaderSize { get; set; }

    }
}
