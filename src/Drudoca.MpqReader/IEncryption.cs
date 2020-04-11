namespace Drudoca.MpqReader
{
    internal interface IEncryption
    {
        uint Hash(string text, int hashType);
        void DecryptInPlace(byte[] data, int offset, int length, uint key);
    }

    internal static class EncryptionExtensions
    {
        public static uint HashTableOffset(this IEncryption @this, string text)
            => @this.Hash(text, 0);

        public static uint HashNameA(this IEncryption @this, string text)
            => @this.Hash(text, 1);

        public static uint HashNameB(this IEncryption @this, string text)
            => @this.Hash(text, 2);

        public static uint HashFileKey(this IEncryption @this, string text)
            => @this.Hash(text, 3);
    }
}