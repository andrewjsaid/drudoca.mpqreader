using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqArchiveHeaderReader : IStructureReader<MpqArchiveHeader?>
    {
        public int InitialSize => 32;

        public async ValueTask<MpqArchiveHeader?> ReadAsync(MpqStreamReaderContext ctx)
        {
            const int maxSupportedVersion = 3;
            const int maxHeaderSize = 208;

            var signature = ctx.ReadInt32();
            if (signature != MpqConstants.MpqHeaderSignature)
            {
                return null;
            }

            var headerSize = ctx.ReadInt32();
            var archiveSize = ctx.ReadInt32();
            var formatVersion = ctx.ReadUInt16();

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

            if (headerSize > ctx.BufferSize)
            {
                await ctx.GrowAsync(headerSize - ctx.BufferSize);
            }

            var blockSize = ctx.ReadUInt16();
            var hashTableOffset = ctx.ReadInt32();
            var blockTableOffset = ctx.ReadInt32();
            var hashTableCount = ctx.ReadInt32();
            var blockTableCount = ctx.ReadInt32();

            if (formatVersion == 0)
            {
                return new MpqArchiveHeader(
                    signature, headerSize, archiveSize,
                    formatVersion, blockSize,
                    hashTableOffset, blockTableOffset,
                    hashTableCount, blockTableCount);
            }

            var hiBlockTableOffset = ctx.ReadInt64();
            var hashTableOffsetHi = ctx.ReadUInt16();
            var blockTableOffsetHi = ctx.ReadUInt16();

            if (formatVersion == 1)
            {
                return new MpqArchiveHeaderV2(
                    signature, headerSize, archiveSize,
                    formatVersion, blockSize,
                    hashTableOffset, blockTableOffset,
                    hashTableCount, blockTableCount,
                    hiBlockTableOffset, hashTableOffsetHi, blockTableOffsetHi);
            }

            var arhiveSize2 = ctx.ReadInt64();
            var betTableOffset = ctx.ReadInt64();
            var hetTableOffset = ctx.ReadInt64();

            if (formatVersion == 2)
            {
                return new MpqArchiveHeaderV3(
                    signature, headerSize, archiveSize,
                    formatVersion, blockSize,
                    hashTableOffset, blockTableOffset,
                    hashTableCount, blockTableCount,
                    hiBlockTableOffset, hashTableOffsetHi, blockTableOffsetHi,
                    arhiveSize2, betTableOffset, hetTableOffset);
            }

            var hashTableSize = ctx.ReadInt64();
            var blockTableSize = ctx.ReadInt64();
            var hiBlockTableSize = ctx.ReadInt64();
            var hetTableSize = ctx.ReadInt64();
            var betTableSize = ctx.ReadInt64();

            var rawChunkSize = ctx.ReadInt32();

            const int md5DigestSize = 16;
            var md5BlockTable = ctx.ReadByteArray(md5DigestSize);
            var md5HashTable = ctx.ReadByteArray(md5DigestSize);
            var md5HiBlockTable = ctx.ReadByteArray(md5DigestSize);
            var md5BetTable = ctx.ReadByteArray(md5DigestSize);
            var md5HetTable = ctx.ReadByteArray(md5DigestSize);
            var md5MpqHeader = ctx.ReadByteArray(md5DigestSize);

            const int lengthOfDataToHash = 208 - 16; // Everything except the last md5;
            var isValid = Md5Validation.IsValid(ctx.Buffer, 0, lengthOfDataToHash, md5MpqHeader);
            if (!isValid)
            {
                throw new InvalidDataException("Incorrect header (based on hash comparison).");
            }

            if (formatVersion == 3)
            {
                return new MpqArchiveHeaderV4(
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
