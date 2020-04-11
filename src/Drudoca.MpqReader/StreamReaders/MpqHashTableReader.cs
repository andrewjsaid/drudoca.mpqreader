using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqHashTableReader
    {
        private IMd5Validation _md5Validation;
        private IEncryption _encryption;

        private const uint _encryptionKey = 0xc3af3770; // HashFileKey("(hash table)")

        public MpqHashTableReader(IMd5Validation md5Validation, IEncryption encryption)
        {
            _md5Validation = md5Validation;
            _encryption = encryption;
        }

        public async Task<MpqHashTable[]> ReadAsync(Stream stream, int count, byte[]? md5)
        {
            const int size = 16;

            using var ctx = new MpqStreamReaderContext(stream);
            await ctx.ReadAsync(size * count);

            if (md5 != null)
            {
                var isValid = _md5Validation.Check(ctx.Buffer, 0, ctx.BufferSize, md5);
                if (!isValid)
                {
                    throw new InvalidDataException("Block table MD5 check failed.");
                }
            }

            _encryption.DecryptInPlace(ctx.Buffer, 0, ctx.BufferSize, _encryptionKey);

            var results = new MpqHashTable[count];

            for (int i = 0; i < count; i++)
            {
                var record = Read(ctx);
                results[i] = record;
            }

            return results;
        }

        private MpqHashTable Read(MpqStreamReaderContext ctx)
        {
            var nameHash1 = ctx.ReadInt32();
            var nameHash2 = ctx.ReadInt32();
            var locale = ctx.ReadUInt16();
            var platform = ctx.ReadUInt16();
            var blockIndex = ctx.ReadInt32();

            return new MpqHashTable(
                nameHash1,
                nameHash2,
                locale,
                platform,
                blockIndex);
        }
    }
}
