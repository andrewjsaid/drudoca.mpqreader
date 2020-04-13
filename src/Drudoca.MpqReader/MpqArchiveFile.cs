using System.Threading.Tasks;
using Drudoca.MpqReader.Extraction;

namespace Drudoca.MpqReader
{
    public class MpqArchiveFile
    {
        private readonly MpqArchive _archive;
        private readonly IFileTableEntry _entry;

        internal MpqArchiveFile(MpqArchive archive, IFileTableEntry entry)
        {
            _archive = archive;
            _entry = entry;
        }

        public long Size => _entry.FileSize;

        public async Task<byte[]> ReadAsync()
        {
            var reader = new DataFileReader();
            return await reader.ReadAsync(_archive, _entry);
        }
    }
}
