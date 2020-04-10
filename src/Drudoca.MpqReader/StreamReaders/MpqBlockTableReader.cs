using System;
using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Headers;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqBlockTableReader : MpqStreamReaderBase<MpqBlockTable>
    {
        public MpqBlockTableReader(Stream stream) : base(stream) { }

        protected override int InitialSize => throw new NotImplementedException();

        protected override ValueTask<MpqBlockTable> ReadAsync(ByteArrayReader bar)
            => new ValueTask<MpqBlockTable>(Read(bar));

        protected MpqBlockTable Read(ByteArrayReader bar)
        {
            var fileOffset = bar.ReadInt32();
            var compressedFileSize = bar.ReadInt32();
            var fileSize = bar.ReadInt32();
            var flags = bar.ReadInt32();

            return new MpqBlockTable(
                fileOffset,
                compressedFileSize,
                fileSize,
                flags);
        }
    }
}
