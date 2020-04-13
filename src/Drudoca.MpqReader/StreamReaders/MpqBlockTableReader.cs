using System;
using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqBlockTableReader
    {
        private readonly IMd5Validation _md5Validation;
        private readonly IEncryption _encryption;

        private const uint _encryptionKey = 0xec83b3a3; // HashFileKey("(block table)")

        public MpqBlockTableReader(IMd5Validation md5Validation, IEncryption encryption)
        {
            _md5Validation = md5Validation;
            _encryption = encryption;
        }

        public async Task<MpqBlockTable[]> ReadAsync(Stream stream, int count, byte[]? md5)
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

            var results = new MpqBlockTable[count];

            for (int i = 0; i < count; i++)
            {
                var record = Read(ctx);
                results[i] = record;
            }

            return results;
        }

        private MpqBlockTable Read(MpqStreamReaderContext ctx)
        {
            var fileOffset = ctx.ReadInt32();
            var compressedFileSize = ctx.ReadInt32();
            var fileSize = ctx.ReadInt32();
            var flags = ctx.ReadUInt32();

            return new MpqBlockTable(
                fileOffset,
                compressedFileSize,
                fileSize,
                flags);
        }
    }
}
