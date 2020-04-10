namespace Drudoca.MpqReader
{
    internal static class MpqConstants
    {

        /// <summary>
        /// Indicates the start of the User Data structure.
        /// Represents 'MPQ\x1B'
        /// i.e. ((byte)'M' + ((byte)'P' << 8) + ((byte)'Q' << 16) + (0x1B << 24)).ToString("X")
        /// </summary>
        public const int MpqUserDataSignature = 0x1B51504D;

        /// <summary>
        /// Indicates the start of the User Data structure.
        /// Represents 'MPQ\x1A'
        /// i.e. ((byte)'M' + ((byte)'P' << 8) + ((byte)'Q' << 16) + (0x1A << 24)).ToString("X")
        /// </summary>
        public const int MpqHeaderSignature = 0x1A51504D;

    }
}
