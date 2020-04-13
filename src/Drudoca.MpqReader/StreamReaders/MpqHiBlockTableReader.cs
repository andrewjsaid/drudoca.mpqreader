using System.IO;
using System.Threading.Tasks;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqHiBlockTableReader
    {
        private readonly IMd5Validation _md5Validation;

        public MpqHiBlockTableReader(IMd5Validation md5Validation)
        {
            _md5Validation = md5Validation;
        }

        public async Task<ushort[]> ReadAsync(Stream stream, int count, byte[]? md5)
        {
            const int size = 2;

            using var context = new MpqStreamReaderContext(stream);
            var r = context.Reader;

            await context.ReadAsync(size * count);

            if (md5 != null)
            {
                var isValid = _md5Validation.Check(context.Buffer, 0, context.BufferSize, md5);
                if (!isValid)
                {
                    throw new InvalidDataException("Block table MD5 check failed.");
                }
            }

            var results = new ushort[count];

            for (int i = 0; i < count; i++)
            {
                var record = r.ReadUInt16();
                results[i] = record;
            }

            return results;
        }
    }
}
