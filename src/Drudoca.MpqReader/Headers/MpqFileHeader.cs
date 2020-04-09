namespace Drudoca.MpqReader
{
    internal class MpqFileHeader
    {
        /// <summary>
        /// The ID_MPQ ('MPQ\x1A') signature
        /// </summary>
        public int Signature { get; }

        /// <summary>
        /// Size of the archive header
        /// </summary>
        public int HeaderSize { get; }

        /// <summary>
        /// Size of MPQ archive
        /// This field is deprecated in the Burning Crusade MoPaQ format, and the size of the archive
        /// is calculated as the size from the beginning of the archive to the end of the hash table,
        /// block table, or extended block table (whichever is largest).
        /// </summary>
        public int ArchiveSize { get; }

        /// <summary>
        /// 0 = Format 1 (up to The Burning Crusade)
        /// 1 = Format 2 (The Burning Crusade and newer)
        /// 2 = Format 3 (WoW - Cataclysm beta or newer)
        /// 3 = Format 4 (WoW - Cataclysm beta or newer)
        /// </summary>
        public ushort FormatVersion { get; }

        /// <summary>
        /// Power of two exponent specifying the number of 512-byte disk sectors in each logical sector
        /// in the archive. The size of each logical sector in the archive is 512 * 2^wBlockSize.
        /// </summary>
        public ushort BlockSize { get; }

        /// <summary>
        /// Offset to the beginning of the hash table, relative to the beginning of the archive.
        /// </summary>
        public int HashTableOffset { get; }

        /// <summary>
        /// Offset to the beginning of the block table, relative to the beginning of the archive.
        /// </summary>
        public int BlockTableOffset { get; }

        /// <summary>
        /// Number of entries in the hash table. Must be a power of two, and must be less than 2^16 for
        /// the original MoPaQ format, or less than 2^20 for the Burning Crusade format.
        /// </summary>
        public int HashTableCount { get; }

        /// <summary>
        /// Number of entries in the block table
        /// </summary>
        public int BlockTableCount { get; }

        //-- MPQ HEADER v 2 -------------------------------------------

        /// <summary>
        /// Offset to the beginning of array of 16-bit high parts of file offsets.
        /// </summary>
        public ulong HiBlockTableOffset { get; }

        /// <summary>
        /// High 16 bits of the hash table offset for large archives.
        /// </summary>
        public ushort HashTableOffsetHi { get; }

        /// <summary>
        /// High 16 bits of the block table offset for large archives.
        /// </summary>
        public ushort BlockTableOffsetHi { get; }

        //-- MPQ HEADER v 3 -------------------------------------------

        /// <summary>
        /// 64-bit version of the archive size
        /// </summary>
        public ulong ArchiveSize2 { get; }

        /// <summary>
        /// 64-bit position of the BET table
        /// </summary>
        public ulong BetTableOffset { get; }

        /// <summary>
        /// 64-bit position of the HET table
        /// </summary>
        public ulong HetTableOffset { get; }

        //-- MPQ HEADER v 4 -------------------------------------------

        /// <summary>
        /// Compressed size of the hash table
        /// </summary>
        public ulong HashTableSize { get; }

        /// <summary>
        /// Compressed size of the block table
        /// </summary>
        public ulong BlockTableSize { get; }

        /// <summary>
        /// Compressed size of the hi-block table
        /// </summary>
        public ulong HiBlockTableSize { get; }

        /// <summary>
        /// Compressed size of the HET block
        /// </summary>
        public ulong HetTableSize { get; }

        /// <summary>
        /// Compressed size of the BET block
        /// </summary>
        public ulong BetTableSize { get; }

        /// <summary>
        /// Size of raw data chunk to calculate MD5.
        /// MD5 of each data chunk follows the raw file data.
        /// </summary>
        public int RawChunkSize { get; }

        /// <summary>
        /// MD5 of the block table before decryption
        /// </summary>
        public string Md5BlockTable { get; }

        /// <summary>
        /// MD5 of the hash table before decryption
        /// </summary>
        public string Md5_HashTable { get; }

        /// <summary>
        /// MD5 of the hi-block table
        /// </summary>
        public string Md5_HiBlockTable { get; }

        /// <summary>
        /// MD5 of the BET table before decryption
        /// </summary>
        public string Md5_BetTable { get; }

        /// <summary>
        /// MD5 of the HET table before decryption
        /// </summary>
        public string Md5_HetTable { get; }

        /// <summary>
        /// MD5 of the MPQ header from signature to (including) MD5_HetTable
        /// </summary>
        public string Md5_MpqHeader { get; }
    }
}
