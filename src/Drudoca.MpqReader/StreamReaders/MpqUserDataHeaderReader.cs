using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqUserDataHeaderReader
    {
        public async Task<MpqUserDataHeader?> ReadAsync(Stream stream)
        {
            using var context= new MpqStreamReaderContext(stream);
            var r = context.Reader;

            await context.ReadAsync(16);

            var signature = r.ReadInt32();
            if (signature != MpqConstants.MpqUserDataSignature)
                return null;

            var userDataSize = r.ReadInt32();
            var headerOffset = r.ReadInt32();
            var userDataHeaderSize = r.ReadInt32();

            return new MpqUserDataHeader(
                userDataSize, headerOffset, userDataHeaderSize);
        }
    }
}
