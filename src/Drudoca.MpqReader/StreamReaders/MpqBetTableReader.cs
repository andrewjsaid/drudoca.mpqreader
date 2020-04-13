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
        private readonly ICrypto _encryption;

        private const uint _encryptionKey = 0xec83b3a3; // HashFileKey("(block table)")

        public MpqBetTableReader(IMd5Validation md5Validation, ICrypto encryption)
        {
            _md5Validation = md5Validation;
            _encryption = encryption;
        }

        public async Task<MpqBetTable?> ReadAsync(Stream stream, byte[]? md5, long? size)
        {
            const int headerSize = 12;

            using var context = new MpqStreamReaderContext(stream);
            var r = context.Reader;

            await context.ReadAsync(headerSize);

            var signature = r.ReadInt32();
            if (signature != MpqConstants.MpqBetTableSignature)
            {
                return null;
            }

            var version = r.ReadInt32();
            if (version != 1)
            {
                throw new NotSupportedException($"Only supporting bet table version 1. Version: {version}");
            }

            var dataSize = r.ReadInt32();
            if (size < dataSize + headerSize)
            {
                throw new NotSupportedException("Compressed Extension Table is not yet supported.");
            }

            await context.ReadAsync(dataSize);

            if (md5 != null)
            {
                var isValid = _md5Validation.Check(context.Buffer, 0, context.BufferSize, md5);
                if (!isValid)
                {
                    throw new InvalidDataException("Bet table MD5 check failed.");
                }
            }

            _encryption.DecryptInPlace(context.Buffer, headerSize, dataSize, _encryptionKey);

            var tableSize = r.ReadInt32();
            // Should be equal to dataSize as far as I understand
            if (dataSize != tableSize)
            {
                throw new InvalidDataException($"DataSize {dataSize} should be equal to TableSize {tableSize}");
            }

            var numEntries = r.ReadInt32();
            var unknownThingy = r.ReadInt32();
            if (unknownThingy != 0x10)
            {
                throw new InvalidDataException($"Unexpected value for <unknown>.");
            }

            var tableEntryBitSize = r.ReadInt32();
            var fileOffsetBitIndex = r.ReadInt32();
            var fileSizeBitIndex = r.ReadInt32();
            var compressedSizeBitIndex = r.ReadInt32();
            var flagIndexBitIndex = r.ReadInt32();
            var unknownBitIndex = r.ReadInt32();
            var fileOffsetBitSize = r.ReadInt32();
            var fileSizeBitSize = r.ReadInt32();
            var compressedSizeBitSize = r.ReadInt32();
            var flagIndexBitSize = r.ReadInt32();
            var unknownBitSize = r.ReadInt32();
            if (unknownBitSize > 0)
            {
                throw new InvalidDataException($"Unknown bit size has always been 0.");
            }

            var totalBetHashBitSize = r.ReadInt32();
            var extraBetHashBitSize = r.ReadInt32();
            var betHashBitSize = r.ReadInt32();
            var betHashTableSize = r.ReadInt32();

            if (betHashTableSize != ((numEntries * totalBetHashBitSize) + 7) / 8)
            {
                throw new InvalidDataException(
                    $"BetHashTableSize {betHashTableSize} should be predictable from NumEntries {numEntries} and TotalBetHashBitSize {totalBetHashBitSize}");
            }

            var flagCount = r.ReadInt32();

            var flags = new uint[flagCount];
            for (int i = 0; i < flagCount; i++)
            {
                flags[i] = r.ReadUInt32();
            }

            var entryTable = new MpqBlockTable[numEntries];
            {
                // Entry Table
                var bitOffset = 0;
                for (int i = 0; i < numEntries; i++)
                {
                    var fileOffset = (long)r.ReadBits(bitOffset + fileOffsetBitIndex, fileOffsetBitSize);
                    var fileSize = (long)r.ReadBits(bitOffset + fileSizeBitIndex, fileSizeBitSize);
                    var compressedFileSize = (long)r.ReadBits(bitOffset + compressedSizeBitIndex, compressedSizeBitSize);
                    Debug.Assert(compressedFileSize <= fileSize);
                    var flagIndex = (int)r.ReadBits(bitOffset + flagIndexBitIndex, flagIndexBitSize);
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
                r.Advance((bitOffset + 7) / 8);
            }

            var betHashTable = new ulong[numEntries];
            {
                // Bet Hashes
                var bitOffset = 0;
                for (int i = 0; i < numEntries; i++)
                {
                    var hash = r.ReadBits(bitOffset, betHashBitSize);
                    betHashTable[i] = hash;
                    bitOffset += totalBetHashBitSize;
                }
                r.Advance(betHashTableSize);
            }

            return new MpqBetTable(numEntries, entryTable, betHashTable);
        }
    }
}
