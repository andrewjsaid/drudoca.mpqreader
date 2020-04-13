using System.Diagnostics;
using System.IO;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader
{
    internal class BasicFileTableBuilder
    {

        public BasicFileTable Build(
            MpqHashTable[] hashTable,
            MpqBlockTable[] blockTable,
            ushort[]? hiBlockTable)
        {
            var entries = new BasicFileTableEntry?[hashTable.Length];
            for (int i = 0; i < entries.Length; i++)
            {
                var entry = BuildEntry(hashTable, blockTable, hiBlockTable, i);
                entries[i] = entry;
            }

            return new BasicFileTable(entries);
        }

        private BasicFileTableEntry? BuildEntry(
            MpqHashTable[] hashTable,
            MpqBlockTable[] blockTable,
            ushort[]? hiBlockTable,
            int i)
        {
            var ht = hashTable[i];

            if (ht.BlockIndex == MpqConstants.HashTableBlockIndexEmpty)
                return null;

            var isDeleted = ht.BlockIndex == MpqConstants.HashTableBlockIndexDeleted;
            if (isDeleted)
            {
                return new BasicFileTableEntry(
                    ht.NameHash1, ht.NameHash2, ht.Locale, ht.Platform, true,
                    0, 0, 0, 0);
            }

            if (ht.BlockIndex < 0 || ht.BlockIndex >= blockTable.Length)
            {
                throw new InvalidDataException($"Invalid BlockIndex: {ht.BlockIndex}. Block Table has {blockTable.Length} records.");
            }

            var bt = blockTable[ht.BlockIndex];

            var fileOffset = bt.FileOffset;
            if (hiBlockTable != null)
            {
                Debug.Assert(hiBlockTable.Length == blockTable.Length);
                var hiOffset = hiBlockTable[ht.BlockIndex];
                fileOffset = ((long)hiOffset << 32) | fileOffset;
            }

            return new BasicFileTableEntry(
                ht.NameHash1, ht.NameHash2, ht.Locale, ht.Platform, false,
                fileOffset, bt.CompressedFileSize, bt.FileSize, (BlockFileFlags)bt.Flags);
        }
    }
}
