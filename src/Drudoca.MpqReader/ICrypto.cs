namespace Drudoca.MpqReader
{
    internal interface ICrypto
    {
        uint Hash(string text, int hashType);
        void DecryptInPlace(byte[] data, int offset, int length, uint key);
    }

    internal static class CryptoExtensions
    {
        public static uint HashTableIndex(this ICrypto @this, string text)
            => @this.Hash(text, 0);

        public static uint HashName1(this ICrypto @this, string text)
            => @this.Hash(text, 1);

        public static uint HashName2(this ICrypto @this, string text)
            => @this.Hash(text, 2);

        public static uint HashFileKey(this ICrypto @this, string text)
            => @this.Hash(text, 3);
    }
}