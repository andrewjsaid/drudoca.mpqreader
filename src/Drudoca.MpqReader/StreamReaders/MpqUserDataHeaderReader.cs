using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqUserDataHeaderReader
    {
        public async Task<MpqUserDataHeader?> ReadAsync(Stream stream)
        {
            using var ctx = new MpqStreamReaderContext(stream);
            await ctx.ReadAsync(16);

            var signature = ctx.ReadInt32();
            if (signature != MpqConstants.MpqUserDataSignature)
                return null;

            var userDataSize = ctx.ReadInt32();
            var headerOffset = ctx.ReadInt32();
            var userDataHeaderSize = ctx.ReadInt32();

            return new MpqUserDataHeader(
                userDataSize, headerOffset, userDataHeaderSize);
        }
    }
}
