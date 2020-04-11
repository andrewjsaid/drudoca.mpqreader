using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqHashTableReader : IStructureReader<MpqHashTable>
    {
        public int InitialSize => 16;

        public ValueTask<MpqHashTable> ReadAsync(MpqStreamReaderContext ctx)
            => new ValueTask<MpqHashTable>(Read(ctx));

        private MpqHashTable Read(MpqStreamReaderContext bar)
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
