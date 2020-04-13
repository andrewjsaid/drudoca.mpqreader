using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqHashTableReader
    {
        private readonly IMd5Validation _md5Validation;
        private readonly ICrypto _encryption;

        private const uint _encryptionKey = 0xc3af3770; // HashFileKey("(hash table)")

        public MpqHashTableReader(IMd5Validation md5Validation, ICrypto encryption)
        {
            _md5Validation = md5Validation;
            _encryption = encryption;
        }

        public async Task<MpqHashTable[]> ReadAsync(Stream stream, int count, byte[]? md5)
        {
            const int size = 16;

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

            _encryption.DecryptInPlace(context.Buffer, 0, context.BufferSize, _encryptionKey);

            var results = new MpqHashTable[count];

            for (int i = 0; i < count; i++)
            {
                var record = Read(r);
                results[i] = record;
            }

            return results;
        }

        private MpqHashTable Read(ByteArrayReader r)
        {
            var nameHash1 = r.ReadUInt32();
            var nameHash2 = r.ReadUInt32();
            var locale = r.ReadUInt16();
            var platform = r.ReadUInt16();
            var blockIndex = r.ReadUInt32();

            return new MpqHashTable(
                nameHash1,
                nameHash2,
                locale,
                platform,
                blockIndex);
        }
    }
}
