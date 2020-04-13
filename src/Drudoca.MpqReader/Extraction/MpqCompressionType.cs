using System;

namespace Drudoca.MpqReader.Extraction
{
    [Flags]
    internal enum MpqCompressionType : byte
    {
        IMAADPCMmono = 0x40,
        IMAADPCMStereo = 0x80,
        HuffmanEncoded = 0x01,
        Deflated = 0x02,
        Imploded = 0x08,
        BZip2 = 0x10
    }
}
