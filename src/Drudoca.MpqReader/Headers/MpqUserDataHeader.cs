namespace Drudoca.MpqReader
{
    internal class MpqUserDataHeader
    {
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
