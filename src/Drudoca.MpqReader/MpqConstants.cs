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

        /// <summary>
        /// Indicates the start of the Het Table.
        /// Represents 'MPQ\x1A'
        /// i.e. ((byte)'H' + ((byte)'E' << 8) + ((byte)'T' << 16) + (0x1A << 24)).ToString("X")
        /// </summary>
        public const int MpqHetTableSignature = 0x1A544548;

        /// <summary>
        /// Indicates the start of the Het Table.
        /// Represents 'MPQ\x1A'
        /// i.e. ((byte)'B' + ((byte)'E' << 8) + ((byte)'T' << 16) + (0x1A << 24)).ToString("X")
        /// </summary>
        public const int MpqBetTableSignature = 0x1A544542;

        public const ushort LanguageNeutral = 0;
        public const ushort LanguageCzech = 0x405;
        public const ushort LanguageEnglish = 0x409;
        public const ushort LanguageFrench = 0x40c;
        public const ushort LanguageJapanese = 0x411;
        public const ushort LanguagePolish = 0x415;
        public const ushort LanguageRussian = 0x419;
        public const ushort LanguageChineseTaiwan = 0x404;
        public const ushort LanguageGerman = 0x407;
        public const ushort LanguageSpanish = 0x40a;
        public const ushort LanguageItalian = 0x410;
        public const ushort LanguageKorean = 0x412;
        public const ushort LanguagePortuguese = 0x416;
        public const ushort LanguageEnglishUK = 0x809;
    }
}
