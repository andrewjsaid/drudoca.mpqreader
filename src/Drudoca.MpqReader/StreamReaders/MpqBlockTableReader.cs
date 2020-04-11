using System;
using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqBlockTableReader : IStructureReader<MpqBlockTable>
    {
        public int InitialSize => 16;

        public ValueTask<MpqBlockTable> ReadAsync(MpqStreamReaderContext ctx)
            => new ValueTask<MpqBlockTable>(Read(ctx));

        private MpqBlockTable Read(MpqStreamReaderContext ctx)
        {
            var fileOffset = ctx.ReadInt32();
            var compressedFileSize = ctx.ReadInt32();
            var fileSize = ctx.ReadInt32();
            var flags = ctx.ReadInt32();

            return new MpqBlockTable(
                fileOffset,
                compressedFileSize,
                fileSize,
                flags);
        }
    }
}
