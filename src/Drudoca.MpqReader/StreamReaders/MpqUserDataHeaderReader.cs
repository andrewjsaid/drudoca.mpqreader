using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqUserDataHeaderReader : IStructureReader<MpqUserDataHeader?>
    {
        public int InitialSize => 16;

        public ValueTask<MpqUserDataHeader?> ReadAsync(MpqStreamReaderContext ctx)
            => new ValueTask<MpqUserDataHeader?>(Read(ctx));

        private MpqUserDataHeader? Read(MpqStreamReaderContext ctx)
        {
            var signature = ctx.ReadInt32();
            if (signature != MpqConstants.MpqUserDataSignature)
                return null;

            var userDataSize = ctx.ReadInt32();
            var headerOffset = ctx.ReadInt32();
            var userDataHeaderSize = ctx.ReadInt32();

            return new MpqUserDataHeader(
                signature, userDataSize,
                headerOffset, userDataHeaderSize);
        }
    }
}
