using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Headers;

namespace Drudoca.MpqReader
{
    internal class MpqFileHeaderStreamReader : MpqStreamReaderBase<MpqFileHeader?>
    {
        public MpqFileHeaderStreamReader(Stream stream) : base(stream) { }

        protected override int InitialSize => 32;

        protected override async ValueTask<MpqFileHeader?> ReadAsync(ByteArrayReader bar)
        {
            const int maxSupportedVersion = 4;
            const int maxHeaderSize = 208;

            var signature = bar.ReadInt32();
            if (signature != MpqConstants.MpqHeaderSignature)
            {
                return null;
            }

            var headerSize = bar.ReadInt32();
            var archiveSize = bar.ReadInt32();
            var formatVersion = bar.ReadUInt16();

            if (formatVersion < 0)
            {
                throw new InvalidDataException($"Unable to read format version (got {formatVersion}).");
            }

            if (formatVersion > maxSupportedVersion)
            {
                throw new NotSupportedException($"Version {formatVersion} is not supported. Support is only up to version {maxSupportedVersion}.");
            }

            if (headerSize > maxHeaderSize)
            {
                throw new InvalidDataException($"Header size {headerSize} is too big.");
            }

            if (headerSize > BufferSize)
            {
                await GrowAsync(headerSize - BufferSize);
            }

            var blockSize = bar.ReadUInt16();
            var hashTableOffset = bar.ReadInt32();
            var blockTableOffset = bar.ReadInt32();
            var hashTableCount = bar.ReadInt32();
            var blockTableCount = bar.ReadInt32();

            if (formatVersion == 1)
            {
                return new MpqFileHeader(
                    signature, headerSize, archiveSize,
                    formatVersion, blockSize,
                    hashTableOffset, blockTableOffset,
                    hashTableCount, blockTableCount);
            }

            var hiBlockTableOffset = bar.ReadUInt64();
            var hashTableOffsetHi = bar.ReadUInt16();
            var blockTableOffsetHi = bar.ReadUInt16();

            if (formatVersion == 2)
            {
                return new MpqFileHeaderV2(
                    signature, headerSize, archiveSize,
                    formatVersion, blockSize,
                    hashTableOffset, blockTableOffset,
                    hashTableCount, blockTableCount,
                    hiBlockTableOffset, hashTableOffsetHi, blockTableOffsetHi);
            }

            var arhiveSize2 = bar.ReadUInt64();
            var betTableOffset = bar.ReadUInt64();
            var hetTableOffset = bar.ReadUInt64();

            if (formatVersion == 3)
            {
                return new MpqFileHeaderV3(
                    signature, headerSize, archiveSize,
                    formatVersion, blockSize,
                    hashTableOffset, blockTableOffset,
                    hashTableCount, blockTableCount,
                    hiBlockTableOffset, hashTableOffsetHi, blockTableOffsetHi,
                    arhiveSize2, betTableOffset, hetTableOffset);
            }

            var hashTableSize = bar.ReadUInt64();
            var blockTableSize = bar.ReadUInt64();
            var hiBlockTableSize = bar.ReadUInt64();
            var hetTableSize = bar.ReadUInt64();
            var betTableSize = bar.ReadUInt64();

            var rawChunkSize = bar.ReadInt32();

            const int md5DigestSize = 16;
            var md5BlockTable = bar.ReadByteArray(md5DigestSize);
            var md5HashTable = bar.ReadByteArray(md5DigestSize);
            var md5HiBlockTable = bar.ReadByteArray(md5DigestSize);
            var md5BetTable = bar.ReadByteArray(md5DigestSize);
            var md5HetTable = bar.ReadByteArray(md5DigestSize);
            var md5MpqHeader = bar.ReadByteArray(md5DigestSize);

            const int lengthOfDataToHash = 208 - 16; // Everything except the last md5;
            var isValid = Md5Validation.IsValid(Buffer, 0, lengthOfDataToHash, md5MpqHeader);
            if (!isValid)
            {
                throw new InvalidDataException("Incorrect header (based on hash comparison).");
            }

            if (formatVersion == 4)
            {
                return new MpqFileHeaderV4(
                    signature, headerSize, archiveSize,
                    formatVersion, blockSize,
                    hashTableOffset, blockTableOffset,
                    hashTableCount, blockTableCount,
                    hiBlockTableOffset, hashTableOffsetHi, blockTableOffsetHi,
                    arhiveSize2, betTableOffset, hetTableOffset,
                    hashTableSize, blockTableSize, hiBlockTableSize,
                    hetTableSize, betTableSize, rawChunkSize,
                    md5BlockTable, md5HashTable, md5HiBlockTable,
                    md5BetTable, md5HetTable, md5MpqHeader);
            }

            Debug.Fail("Should catch wrong versions earlier");
            throw new InvalidOperationException("Should have validated for unsupported version sooner.");
        }
    }
}
