
using System;
using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqHetTableReader
    {
        private readonly IMd5Validation _md5Validation;
        private readonly ICrypto _encryption;

        private const uint _encryptionKey = 0xc3af3770; // HashFileKey("(hash table)")

        public MpqHetTableReader(IMd5Validation md5Validation, ICrypto encryption)
        {
            _md5Validation = md5Validation;
            _encryption = encryption;
        }

        public async Task<MpqHetTable?> ReadAsync(Stream stream, byte[]? md5, long? size)
        {
            const int headerSize = 12;

            using var context = new MpqStreamReaderContext(stream);
            var r = context.Reader;

            await context.ReadAsync(headerSize);

            var signature = r.ReadInt32();
            if (signature != MpqConstants.MpqHetTableSignature)
            {
                return null;
            }

            var version = r.ReadInt32();
            if (version != 1)
            {
                throw new NotSupportedException($"Only supporting het table version 1. Version: {version}");
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
                    throw new InvalidDataException("Het table MD5 check failed.");
                }
            }

            _encryption.DecryptInPlace(context.Buffer, headerSize, dataSize, _encryptionKey);

            var tableSize = r.ReadInt32();
            // Should be equal to dataSize as far as I understand
            if (dataSize != tableSize)
            {
                throw new InvalidDataException($"DataSize {dataSize} should be equal to TableSize {tableSize}");
            }

            var numUsedEntries = r.ReadInt32();
            var numEntries = r.ReadInt32();
            var hashEntryBitSize = r.ReadInt32();
            var totalBetIndexBitSize = r.ReadInt32();
            var extraBetIndexBitSize = r.ReadInt32();
            var betIndexBitSize = r.ReadInt32();
            var betIndexTableSize = r.ReadInt32();

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

            // First there's the name hashes (each entry is a byte)
            var nameHashes = new byte[numEntries];
            for (int i = 0; i < numEntries; i++)
            {
                nameHashes[i] = r.ReadByte();
            }

            var betIndices = ReadBetIndices(r, numEntries, betIndexBitSize, totalBetIndexBitSize);

            return new MpqHetTable(
                numUsedEntries, numEntries, nameHashes, betIndices);
        }

        private long[] ReadBetIndices(ByteArrayReader r, int count, int bitSize, int totalBitSize)
        {
            var results = new long[count];

            var offset = 0;

            for (int i = 0; i < count; i++)
            {
                var betTableIndex = r.ReadBits(offset, bitSize);
                results[i] = (long)betTableIndex;
                offset += totalBitSize;
            }

            return results;
        }
    }
}
