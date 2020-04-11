namespace Drudoca.MpqReader
{
    internal interface IMd5Validation
    {
        bool Check(byte[] data, int offset, int length, byte[] hash);
    }
}