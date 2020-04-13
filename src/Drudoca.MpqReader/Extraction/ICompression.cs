using System;

namespace Drudoca.MpqReader.Extraction
{
    internal interface ICompression
    {
        int Decompress(Memory<byte> source, Memory<byte> target);
    }
}
