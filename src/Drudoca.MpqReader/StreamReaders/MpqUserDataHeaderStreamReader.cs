using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Headers;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqUserDataHeaderStreamReader : MpqStreamReaderBase<MpqUserDataHeader?>
    {
        public MpqUserDataHeaderStreamReader(Stream stream) : base(stream) { }

        protected override int InitialSize => 16;

        protected override ValueTask<MpqUserDataHeader?> ReadAsync(ByteArrayReader bar)
            => new ValueTask<MpqUserDataHeader?>(Read(bar));

        protected MpqUserDataHeader? Read(ByteArrayReader bar)
        {
            var signature = bar.ReadInt32();
            if (signature != MpqConstants.MpqUserDataSignature)
                return null;

            var userDataSize = bar.ReadInt32();
            var headerOffset = bar.ReadInt32();
            var userDataHeaderSize = bar.ReadInt32();

            return new MpqUserDataHeader(
                signature, userDataSize,
                headerOffset, userDataHeaderSize);
        }
    }
}
