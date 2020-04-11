namespace Drudoca.MpqReader.Structures
{
    internal class MpqArchiveHeaderV4 : MpqArchiveHeaderV3
    {

        public MpqArchiveHeaderV4(
            int signature,
            int headerSize,
            int archiveSize,
            ushort formatVersion,
            ushort blockSize,
            int hashTableOffset,
            int blockTableOffset,
            int hashTableCount,
            int blockTableCount,
            long hiBlockTableOffset,
            ushort hashTableOffsetHi,
            ushort blockTableOffsetHi,
            long archiveSize2,
            long betTableOffset,
            long hetTableOffset,
            long hashTableSize,
            long blockTableSize,
            long hiBlockTableSize,
            long hetTableSize,
            long betTableSize,
            int rawChunkSize,
            byte[] md5BlockTable,
            byte[] md5HashTable,
            byte[] md5HiBlockTable,
            byte[] md5BetTable,
            byte[] md5HetTable,
            byte[] md5MpqHeader)
            : base(signature, headerSize, archiveSize,
                   formatVersion, blockSize,
                   hashTableOffset, blockTableOffset,
                   hashTableCount, blockTableCount,
                   hiBlockTableOffset, hashTableOffsetHi, blockTableOffsetHi,
                   archiveSize2, betTableOffset, hetTableOffset)
        {
            HashTableSize = hashTableSize;
            BlockTableSize = blockTableSize;
            HiBlockTableSize = hiBlockTableSize;
            HetTableSize = hetTableSize;
            BetTableSize = betTableSize;
            RawChunkSize = rawChunkSize;
            Md5BlockTable = md5BlockTable;
            Md5HashTable = md5HashTable;
            Md5HiBlockTable = md5HiBlockTable;
            Md5BetTable = md5BetTable;
            Md5HetTable = md5HetTable;
            Md5MpqHeader = md5MpqHeader;
        }

        /// <summary>
        /// Compressed size of the hash table
        /// </summary>
        public long HashTableSize { get; }

        /// <summary>
        /// Compressed size of the block table
        /// </summary>
        public long BlockTableSize { get; }

        /// <summary>
        /// Compressed size of the hi-block table
        /// </summary>
        public long HiBlockTableSize { get; }

        /// <summary>
        /// Compressed size of the HET block
        /// </summary>
        public long HetTableSize { get; }

        /// <summary>
        /// Compressed size of the BET block
        /// </summary>
        public long BetTableSize { get; }

        /// <summary>
        /// Size of raw data chunk to calculate MD5.
        /// MD5 of each data chunk follows the raw file data.
        /// </summary>
        public int RawChunkSize { get; }

        /// <summary>
        /// MD5 of the block table before decryption
        /// </summary>
        public byte[] Md5BlockTable { get; }

        /// <summary>
        /// MD5 of the hash table before decryption
        /// </summary>
        public byte[] Md5HashTable { get; }

        /// <summary>
        /// MD5 of the hi-block table
        /// </summary>
        public byte[] Md5HiBlockTable { get; }

        /// <summary>
        /// MD5 of the BET table before decryption
        /// </summary>
        public byte[] Md5BetTable { get; }

        /// <summary>
        /// MD5 of the HET table before decryption
        /// </summary>
        public byte[] Md5HetTable { get; }

        /// <summary>
        /// MD5 of the MPQ header from signature to (including) MD5_HetTable
        /// </summary>
        public byte[] Md5MpqHeader { get; }
    }
}
