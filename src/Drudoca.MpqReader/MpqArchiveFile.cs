using System.Buffers;
using System.Threading.Tasks;

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

        private bool HasFlag(BlockFileFlags flag) => (_entry.Flags & flag) == flag;

        public async Task<byte[]> ReadAsync()
        {
            var sectors = await ReadSectorsTableAsync();

            var fileData = new byte[_entry.FileSize];
            var fileIndex = 0;

            foreach (var (offset, size) in sectors)
            {
                await ReadSectorAsync(fileData, fileIndex, offset, size);
            }

            return fileData;
        }

        private async Task<(int offset, int size)[]> ReadSectorsTableAsync()
        {
            _archive.Seek(_entry.FileOffset);
            
            var blockSize = (int)_entry.CompressedFileSize;
            var buffer = ArrayPool<byte>.Shared.Rent(blockSize);
            try
            {
                await _archive.ReadAsync(buffer, 0, buffer.Length);
                var r = new ByteArrayReader(buffer, 0);

                if (HasFlag(BlockFileFlags.SingleUnit))
                {
                    return new[] {
                        (0, (int)_entry.CompressedFileSize)
                    };
                }
                else
                {

                }
                return null!;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        private async Task ReadSectorAsync(byte[] fileData, int fileIndex, int offset, int size)
        {
            _archive.Seek(_entry.FileOffset + offset);

            // TODO AJS: Compressed?
            await Task.CompletedTask;
        }
    }
}
