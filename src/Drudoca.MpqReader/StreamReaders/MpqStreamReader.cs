using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    internal sealed class MpqStreamReader
    {
        private async Task<T> ReadAsync<T>(Stream stream, IStructureReader<T> reader)
        {
            using var context = new MpqStreamReaderContext(stream, reader.InitialSize);
            await context.SetupAsync();
            var result = await reader.ReadAsync(context);
            return result;
        }

        private async Task<T[]> ReadManyAsync<T>(Stream stream, int count, IStructureReader<T> reader)
        {
            using var context = new MpqStreamReaderContext(stream, reader.InitialSize * count);
            await context.SetupAsync();
            var results = new T[count];
            for (int i = 0; i < count; i++)
            {
                var t = await reader.ReadAsync(context);
                results[i] = t;
            }
            return results;
        }

        public Task<MpqArchiveHeader?> ReadArchiveHeaderAsync(Stream stream)
            => ReadAsync(stream, new MpqArchiveHeaderReader());

        public Task<MpqUserDataHeader?> ReadUserDataHeaderAsync(Stream stream)
            => ReadAsync(stream, new MpqUserDataHeaderReader());

        public Task<MpqBlockTable[]> ReadBlockTableAsync(Stream stream, int count)
            => ReadManyAsync(stream, count, new MpqBlockTableReader());

        public Task<MpqHashTable[]> ReadHashTableAsync(Stream stream, int count)
            => ReadManyAsync(stream, count, new MpqHashTableReader());

        public Task<MpqBetTable> ReadBetTableAsync(Stream stream)
            => ReadAsync(stream, new MpqBetTableReader());

        public Task<MpqHetTable?> ReadHetTableAsync(Stream stream)
            => ReadAsync(stream, new MpqHetTableReader());
    }
}
