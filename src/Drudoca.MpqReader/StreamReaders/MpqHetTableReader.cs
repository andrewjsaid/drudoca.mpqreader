
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

            var numEntries = ctx.ReadInt32();
            var numSlots = ctx.ReadInt32();
            var hashEntryBitSize = ctx.ReadInt32();
            var totalIndexBitSize = ctx.ReadInt32();
            var indexExtraBitSize = ctx.ReadInt32();
            var indexBitSize = ctx.ReadInt32();
            var indexTableSize = ctx.ReadInt32();

            if (dataSize != (8 * 4) + numSlots + indexTableSize)
            {
                throw new InvalidDataException(
                    $"TableSize {tableSize} should be equal to HeaderSize (32) + NumSlots {numSlots} + IndexTableSize {indexTableSize}");
            }

            if (indexTableSize != ((numSlots * totalIndexBitSize) + 7) / 8)
            {
                throw new InvalidDataException(
                    $"IndexTableSize {indexTableSize} should be predictable from NumSlots {numSlots} and TotalIndexBitSize {totalIndexBitSize}");
            }

            if (totalIndexBitSize != indexBitSize + indexExtraBitSize)
            {
                throw new InvalidDataException(
                    $"TotalIndexBitSize {totalIndexBitSize} must be equal to IndexBitSize {indexBitSize} + IndexExtraBitSize {indexExtraBitSize}");
            }

            if (indexExtraBitSize != 0)
            {
                throw new NotSupportedException("Not sure what this means");
            }

            // First there's the Hash Table (each entry is a byte)
            var hashTable = new byte[numSlots];
            for (int i = 0; i < numSlots; i++)
            {
                hashTable[i] = ctx.ReadByte();
            }

            var fileIndices = ReadFileIndices(ctx, indexTableSize, numSlots, totalIndexBitSize);

            return new MpqHetTable(
                signature, version,
                dataSize, tableSize,
                numEntries, numSlots, hashEntryBitSize,
                totalIndexBitSize, indexExtraBitSize, indexBitSize,
                indexTableSize, hashTable, fileIndices);
        }

        private long[] ReadFileIndices(MpqStreamReaderContext ctx, int numBytesAvailable, int count, int totalIndexBitSize)
        {
            var results = new long[count];

            var q = new BitQueue(0, 0);

            for (int i = 0; i < count; i++)
            {
                long r = 0;

                var bitsRemaining = totalIndexBitSize;

                while (bitsRemaining > 0)
                {
                    if (q.Length == 0)
                    {
                        if(numBytesAvailable == 0)
                        {
                            throw new InvalidDataException("Not enough bytes available to read");
                        }
                        int toRead = Math.Min(numBytesAvailable, 8);
                        for (int n = 0; n < toRead; n++)
                        {
                            q.Append(ctx.ReadByte());
                        }
                        numBytesAvailable -= toRead;
                    }

                    if (q.Length <= bitsRemaining)
                    {
                        r <<= q.Length;
                        bitsRemaining -= q.Length;
                        r |= q.TakeAll();
                    }
                    else // q.Length > bitsRemaining
                    {
                        r <<= bitsRemaining;
                        r |= q.Take(bitsRemaining);
                        bitsRemaining = 0;
                    }
                }

                results[i] = r;
            }

            if (numBytesAvailable != 0)
            {
                throw new InvalidDataException("Did not read all available bytes.");
            }

            // Don't know why, but q.Data can be non-zero

            return results;
        }
    }
}
