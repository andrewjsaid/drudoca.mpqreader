using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Headers;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqHashTableReader : MpqStreamReaderBase<MpqHashTable>
    {
        public MpqHashTableReader(Stream stream) : base(stream) { }

        protected override int InitialSize => 16;

        protected override ValueTask<MpqHashTable> ReadAsync(ByteArrayReader bar)
            => new ValueTask<MpqHashTable>(Read(bar));

        protected MpqHashTable Read(ByteArrayReader bar)
        {
            var nameHash1 = bar.ReadInt32();
            var nameHash2 = bar.ReadInt32();
            var locale = bar.ReadUInt16();
            var platform = bar.ReadUInt16();
            var blockIndex = bar.ReadInt32();
            return new MpqHashTable(
                nameHash1, nameHash2,
                locale, platform, blockIndex);
        }
    }
}
