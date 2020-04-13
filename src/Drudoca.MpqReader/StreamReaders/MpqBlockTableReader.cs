using System;
using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqBlockTableReader
    {
        private readonly IMd5Validation _md5Validation;
        private readonly ICrypto _encryption;

        private const uint _encryptionKey = 0xec83b3a3; // HashFileKey("(block table)")

        public MpqBlockTableReader(IMd5Validation md5Validation, ICrypto encryption)
        {
            _md5Validation = md5Validation;
            _encryption = encryption;
        }

        public async Task<MpqBlockTable[]> ReadAsync(Stream stream, int count, byte[]? md5)
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

            var results = new MpqBlockTable[count];

            for (int i = 0; i < count; i++)
            {
                var record = Read(r);
                results[i] = record;
            }

            return results;
        }

        private MpqBlockTable Read(ByteArrayReader r)
        {
            var fileOffset = r.ReadInt32();
            var compressedFileSize = r.ReadInt32();
            var fileSize = r.ReadInt32();
            var flags = r.ReadUInt32();

            return new MpqBlockTable(
                fileOffset,
                compressedFileSize,
                fileSize,
                flags);
        }
    }
}
