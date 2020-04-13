
using System;
using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqHetTableReader
    {
        private IMd5Validation _md5Validation;
        private IEncryption _encryption;

        private const uint _encryptionKey = 0xc3af3770; // HashFileKey("(hash table)")

        public MpqHetTableReader(IMd5Validation md5Validation, IEncryption encryption)
        {
            _md5Validation = md5Validation;
            _encryption = encryption;
        }

        public async Task<MpqHetTable?> ReadAsync(Stream stream, byte[]? md5)
        {
            const int headerSize = 12;

            using var ctx = new MpqStreamReaderContext(stream);
            await ctx.ReadAsync(headerSize);

            var signature = ctx.ReadInt32();
            if (signature != MpqConstants.MpqHetTableSignature)
            {
                return null;
            }

            var version = ctx.ReadInt32();
            if (version != 1)
            {
                throw new NotSupportedException($"Only supporting het table version 1. Version: {version}");
            }

            var dataSize = ctx.ReadInt32();
            await ctx.ReadAsync(dataSize);

            if (md5 != null)
            {
                var isValid = _md5Validation.Check(ctx.Buffer, 0, ctx.BufferSize, md5);
                if (!isValid)
                {
                    throw new InvalidDataException("Het table MD5 check failed.");
                }
            }

            _encryption.DecryptInPlace(ctx.Buffer, headerSize, dataSize, _encryptionKey);

            var tableSize = ctx.ReadInt32();
            // Should be equal to dataSize as far as I understand
            if (dataSize != tableSize)
            {
                throw new InvalidDataException($"DataSize {dataSize} should be equal to TableSize {tableSize}");
            }

            var numUsedEntries = ctx.ReadInt32();
            var numEntries = ctx.ReadInt32();
            var hashEntryBitSize = ctx.ReadInt32();
            var totalBetIndexBitSize = ctx.ReadInt32();
            var extraBetIndexBitSize = ctx.ReadInt32();
            var betIndexBitSize = ctx.ReadInt32();
            var betIndexTableSize = ctx.ReadInt32();

            if (dataSize != (8 * 4) + numEntries + betIndexTableSize)
            {
                throw new InvalidDataException(
                    $"TableSize {tableSize} should be equal to HeaderSize (32) + NumSlots {numEntries} + betIndexTableSize {betIndexTableSize}");
            }

            if (betIndexTableSize != ((numEntries * totalBetIndexBitSize) + 7) / 8)
            {
                throw new InvalidDataException(
                    $"betIndexTableSize {betIndexTableSize} should be predictable from NumSlots {numEntries} and totalBetIndexBitSize {totalBetIndexBitSize}");
            }

            if (totalBetIndexBitSize != betIndexBitSize + extraBetIndexBitSize)
            {
                throw new InvalidDataException(
                    $"TotalIndexBitSize {totalBetIndexBitSize} must be equal to IndexBitSize {betIndexBitSize} + extraBetIndexBitSize {extraBetIndexBitSize}");
            }

            if (extraBetIndexBitSize != 0)
            {
                throw new NotSupportedException("Extra bits are not supported");
            }

            // First there's the Hash Table (each entry is a byte)
            var hashTable = new byte[numEntries];
            for (int i = 0; i < numEntries; i++)
            {
                hashTable[i] = ctx.ReadByte();
            }

            var betIndices = ReadBetIndices(ctx, betIndexTableSize, numEntries, totalBetIndexBitSize);

            return new MpqHetTable(
                numUsedEntries, numEntries, hashTable, betIndices);
        }

        private long[] ReadBetIndices(MpqStreamReaderContext ctx, int count, int bitSize, int totalBitSize)
        {
            var results = new long[count];

            var offset = 0;

            for (int i = 0; i < count; i++)
            {
                var r = ctx.ReadBits(offset, bitSize);
                results[i] = (long)r;
                offset += totalBitSize;
            }

            return results;
        }
    }
}
