namespace Drudoca.MpqReader
{
    internal interface IEncryption
    {
        void DecryptInPlace(byte[] data, int offset, int length, uint key);
    }
}