using System.IO;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader
{
    internal class ExFileTableBuilder
    {

        public ExFileTable Build(
            MpqHetTable hetTable,
            MpqBetTable betTable)
        {
            var entries = new ExFileTableEntry?[hetTable.NumEntries];
            for (int i = 0; i < entries.Length; i++)
            {
                var entry = BuildEntry(hetTable, betTable, i);
                entries[i] = entry;
            }

            return new ExFileTable(entries);
        }

        private ExFileTableEntry? BuildEntry(
            MpqHetTable hetTable,
            MpqBetTable betTable,
            int hetIndex)
        {
            var hetHash = hetTable.NameHashes[hetIndex];
            if (hetHash == 0)
                return null;

            var betIndex = hetTable.BetIndices[hetIndex];
            if (betIndex < 0 || betIndex >= betTable.NumEntries)
            {
                throw new InvalidDataException($"Invalid BetIndex: {betIndex}. Bet Table has {betTable.NumEntries} records.");
            }

            var betHash = betTable.BetHashTable[betIndex];
            var betBlock = betTable.BlockTable[betIndex];

            var result = new ExFileTableEntry(
                hetHash, betHash,
                betBlock.FileOffset,
                betBlock.CompressedFileSize,
                betBlock.FileSize,
                (BlockFileFlags)betBlock.Flags);

            return result;
        }
    }
}
