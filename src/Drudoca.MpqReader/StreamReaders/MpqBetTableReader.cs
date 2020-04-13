using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqBetTableReader
    {
        private readonly IMd5Validation _md5Validation;
        private readonly IEncryption _encryption;

        private const uint _encryptionKey = 0xec83b3a3; // HashFileKey("(block table)")

        public MpqBetTableReader(IMd5Validation md5Validation, IEncryption encryption)
        {
            _md5Validation = md5Validation;
            _encryption = encryption;
        }

        public async Task<MpqBetTable?> ReadAsync(Stream stream, byte[]? md5, long? size)
        {
            const int headerSize = 12;

            using var ctx = new MpqStreamReaderContext(stream);
            await ctx.ReadAsync(headerSize);

            var signature = ctx.ReadInt32();
            if (signature != MpqConstants.MpqBetTableSignature)
            {
                return null;
            }

            var version = ctx.ReadInt32();
            if (version != 1)
            {
                throw new NotSupportedException($"Only supporting bet table version 1. Version: {version}");
            }

            var dataSize = ctx.ReadInt32();
            if (size < dataSize + headerSize)
            {
                throw new NotSupportedException("Compressed Extension Table is not yet supported.");
            }

            await ctx.ReadAsync(dataSize);

            if (md5 != null)
            {
                var isValid = _md5Validation.Check(ctx.Buffer, 0, ctx.BufferSize, md5);
                if (!isValid)
                {
                    throw new InvalidDataException("Bet table MD5 check failed.");
                }
            }

            _encryption.DecryptInPlace(ctx.Buffer, headerSize, dataSize, _encryptionKey);

            var tableSize = ctx.ReadInt32();
            // Should be equal to dataSize as far as I understand
            if (dataSize != tableSize)
            {
                throw new InvalidDataException($"DataSize {dataSize} should be equal to TableSize {tableSize}");
            }

            var numEntries = ctx.ReadInt32();
            var unknownThingy = ctx.ReadInt32();
            if (unknownThingy != 0x10)
            {
                throw new InvalidDataException($"Unexpected value for <unknown>.");
            }

            var tableEntryBitSize = ctx.ReadInt32();
            var fileOffsetBitIndex = ctx.ReadInt32();
            var fileSizeBitIndex = ctx.ReadInt32();
            var compressedSizeBitIndex = ctx.ReadInt32();
            var flagIndexBitIndex = ctx.ReadInt32();
            var unknownBitIndex = ctx.ReadInt32();
            var fileOffsetBitSize = ctx.ReadInt32();
            var fileSizeBitSize = ctx.ReadInt32();
            var compressedSizeBitSize = ctx.ReadInt32();
            var flagIndexBitSize = ctx.ReadInt32();
            var unknownBitSize = ctx.ReadInt32();
            if (unknownBitSize > 0)
            {
                throw new InvalidDataException($"Unknown bit size has always been 0.");
            }

            var totalBetHashBitSize = ctx.ReadInt32();
            var extraBetHashBitSize = ctx.ReadInt32();
            var betHashBitSize = ctx.ReadInt32();
            var betHashTableSize = ctx.ReadInt32();

            if (betHashTableSize != ((numEntries * totalBetHashBitSize) + 7) / 8)
            {
                throw new InvalidDataException(
                    $"BetHashTableSize {betHashTableSize} should be predictable from NumEntries {numEntries} and TotalBetHashBitSize {totalBetHashBitSize}");
            }

            var flagCount = ctx.ReadInt32();

            var flags = new uint[flagCount];
            for (int i = 0; i < flagCount; i++)
            {
                flags[i] = ctx.ReadUInt32();
            }

            var entryTable = new MpqBlockTable[numEntries];
            {
                // Entry Table
                var bitOffset = 0;
                for (int i = 0; i < numEntries; i++)
                {
                    var fileOffset = (long)ctx.ReadBits(bitOffset + fileOffsetBitIndex, fileOffsetBitSize);
                    var fileSize = (long)ctx.ReadBits(bitOffset + fileSizeBitIndex, fileSizeBitSize);
                    var compressedFileSize = (long)ctx.ReadBits(bitOffset + compressedSizeBitIndex, compressedSizeBitSize);
                    Debug.Assert(compressedFileSize <= fileSize);
                    var flagIndex = (int)ctx.ReadBits(bitOffset + flagIndexBitIndex, flagIndexBitSize);
                    Debug.Assert(flagCount == 0 || flagIndex < flagCount);
                    var eFlags = flagCount > 0 ? flags[flagIndex] : default;

                    var e = new MpqBlockTable(
                        fileOffset,
                        compressedFileSize,
                        fileSize,
                        eFlags);

                    entryTable[i] = e;
                    bitOffset += tableEntryBitSize;
                }
                ctx.Advance((bitOffset + 7) / 8);
            }

            var betHashTable = new ulong[numEntries];
            {
                // Bet Hashes
                var bitOffset = 0;
                for (int i = 0; i < numEntries; i++)
                {
                    var hash = ctx.ReadBits(bitOffset, betHashBitSize);
                    betHashTable[i] = hash;
                    bitOffset += totalBetHashBitSize;
                }
                ctx.Advance(betHashTableSize);
            }

            return new MpqBetTable(numEntries, entryTable, betHashTable);
        }
    }
}
