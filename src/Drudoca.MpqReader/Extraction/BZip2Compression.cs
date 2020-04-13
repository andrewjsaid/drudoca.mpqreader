using System;
using ICSharpCode.SharpZipLib.BZip2;

namespace Drudoca.MpqReader.Extraction
{
    internal class BZip2Compression : ICompression
    {
        public int Decompress(Memory<byte> source, Memory<byte> target)
        {
            using (var msSource = new CustomMemoryStream(source))
            using (var msTarget = new CustomMemoryStream(target))
            {
                BZip2.Decompress(msSource, msTarget, false);
                return (int)msTarget.Position;
            }
        }
    }
}
