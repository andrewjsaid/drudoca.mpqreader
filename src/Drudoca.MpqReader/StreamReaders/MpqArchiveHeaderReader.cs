using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqArchiveHeaderReader
    {
        private IMd5Validation _md5Validation;

        public MpqArchiveHeaderReader(IMd5Validation md5Validation)
        {
            _md5Validation = md5Validation;
        }

        public async Task<MpqArchiveHeader?> ReadAsync(Stream stream)
        {
            const int initialSize = 32;
            const int maxSupportedVersion = 3;
            const int maxHeaderSize = 208;

            using var ctx = new MpqStreamReaderContext(stream);
            await ctx.ReadAsync(initialSize);

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
                await ctx.ReadAsync(headerSize - ctx.BufferSize);
            }

            var blockSize = ctx.ReadUInt16();
            var hashTableOffset = ctx.ReadInt32();
            var blockTableOffset = ctx.ReadInt32();
            var hashTableCount = ctx.ReadInt32();
            var blockTableCount = ctx.ReadInt32();

            if (formatVersion == 0)
            {
                return new MpqArchiveHeader(
                    archiveSize,
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
                    archiveSize,
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
                    archiveSize,
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
            var isValid = _md5Validation.Check(ctx.Buffer, 0, lengthOfDataToHash, md5MpqHeader);
            if (!isValid)
            {
                throw new InvalidDataException("Block table MD5 check failed.");
            }

            if (formatVersion == 3)
            {
                return new MpqArchiveHeaderV4(
                    archiveSize,
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
