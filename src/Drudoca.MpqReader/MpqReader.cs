using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Drudoca.MpqReader.StreamReaders;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader
{
    public class MpqReader
    {

        public async Task<MpqArchive> ReadAsync(Stream stream, bool disposeStream)
        {
            if (!BitConverter.IsLittleEndian)
            {
                throw new NotSupportedException("This library only works on LittleEndian systems.");
            }

            var encryption = new Crypto();
            var sr = new MpqStreamReader(new Md5Validation(), encryption);

            long userDataHeaderOffset = 0;

            var userDataHeader = await sr.ReadUserDataHeaderAsync(stream);
            if (userDataHeader == null)
            {
                // TODO: Search for user header, or allow file to start without user data.
                throw new NotImplementedException("File must start with user data header.");
            }

            var archiveHeaderOffset = userDataHeaderOffset + userDataHeader.HeaderOffset;
            stream.Seek(archiveHeaderOffset, SeekOrigin.Begin);
            var archiveHeader = await sr.ReadArchiveHeaderAsync(stream);
            if (archiveHeader == null)
            {
                // TODO: Later we must support files without UserDataHeader telling us where this one is.
                throw new InvalidDataException("FileHeader not found.");
            }

            var hetTable = await ReadHetTableAsync(stream, archiveHeaderOffset, sr, archiveHeader);
            var betTable = await ReadBetTableAsync(stream, archiveHeaderOffset, sr, archiveHeader);
            var hashTable = await ReadHashTableAsync(stream, archiveHeaderOffset, sr, archiveHeader);
            var blockTable = await ReadBlockTableAsync(stream, archiveHeaderOffset, sr, archiveHeader);
            var hiBlockTable = await ReadHiBlockTableAsync(stream, archiveHeaderOffset, sr, archiveHeader);

            BasicFileTable? basicFileTable = null;
            if (hashTable != null && blockTable != null)
            {
                var builder = new BasicFileTableBuilder();
                basicFileTable = builder.Build(hashTable, blockTable, hiBlockTable);
            }
            else if ((hashTable == null) != (blockTable == null))
            {
                throw new InvalidDataException("Hash Table and Block Table must either both be null or neither");
            }


            ExFileTable? exFileTable = null;
            if (hetTable != null && betTable != null)
            {
                var builder = new ExFileTableBuilder();
                exFileTable = builder.Build(hetTable, betTable);
            }
            else if ((hetTable == null) != (blockTable == null))
            {
                throw new InvalidDataException("Het Table and Bet Table must either both be null or neither");
            }

            // TODO: Verify digital signature.

            var result = new MpqArchive(
                stream, disposeStream,
                userDataHeaderOffset, userDataHeader,
                archiveHeaderOffset, archiveHeader,
                basicFileTable, exFileTable);
            
            result.Crypto = encryption;

            return result;
        }

        private async Task<MpqHetTable?> ReadHetTableAsync(
            Stream stream,
            long archiveOffset,
            MpqStreamReader sr,
            MpqArchiveHeader archiveHeader)
        {
            if (archiveHeader is MpqArchiveHeaderV3 archiveHeader3)
            {
                long tableOffset = archiveHeader3.HetTableOffset;
                if (tableOffset == 0)
                    return null;

                byte[]? md5 = null;
                long? size = null;
                if (archiveHeader is MpqArchiveHeaderV4 archiveHeader4)
                {
                    md5 = archiveHeader4.Md5HetTable;
                    size = archiveHeader4.HetTableSize;
                }

                stream.Seek(archiveOffset + tableOffset, SeekOrigin.Begin);
                var result = await sr.ReadHetTableAsync(stream, md5, size);

                return result;
            }
            else
            {
                return null;
            }
        }

        private async Task<MpqBetTable?> ReadBetTableAsync(
            Stream stream,
            long archiveOffset,
            MpqStreamReader sr,
            MpqArchiveHeader archiveHeader)
        {
            if (archiveHeader is MpqArchiveHeaderV3 archiveHeader3)
            {
                long tableOffset = archiveHeader3.BetTableOffset;
                if (tableOffset == 0)
                    return null;

                byte[]? md5 = null;
                long? size = null;
                if (archiveHeader is MpqArchiveHeaderV4 archiveHeader4)
                {
                    md5 = archiveHeader4.Md5BetTable;
                    size = archiveHeader4.BetTableSize;
                }

                stream.Seek(archiveOffset + tableOffset, SeekOrigin.Begin);
                var result = await sr.ReadBetTableAsync(stream, md5, size);

                return result;
            }
            else
            {
                return null;
            }
        }

        private async Task<MpqHashTable[]?> ReadHashTableAsync(
            Stream stream,
            long archiveOffset,
            MpqStreamReader sr,
            MpqArchiveHeader archiveHeader)
        {
            long tableOffset = archiveHeader.HashTableOffset;
            if (tableOffset == 0)
                return null;

            if (archiveHeader is MpqArchiveHeaderV2 archiveHeader2)
            {
                tableOffset = ((long)archiveHeader2.HashTableOffsetHi << 32) | tableOffset;
            }

            var md5 = (archiveHeader as MpqArchiveHeaderV4)?.Md5HashTable;

            stream.Seek(archiveOffset + tableOffset, SeekOrigin.Begin);
            var result = await sr.ReadHashTableAsync(stream, archiveHeader.HashTableCount, md5);
            return result;
        }

        private async Task<MpqBlockTable[]?> ReadBlockTableAsync(
            Stream stream,
            long archiveOffset,
            MpqStreamReader sr,
            MpqArchiveHeader archiveHeader)
        {
            long tableOffset = archiveHeader.BlockTableOffset;
            if (tableOffset == 0)
                return null;

            if (archiveHeader is MpqArchiveHeaderV2 archiveHeader2)
            {
                tableOffset = ((long)archiveHeader2.BlockTableOffsetHi << 32) | tableOffset;
            }

            var md5 = (archiveHeader as MpqArchiveHeaderV4)?.Md5BlockTable;

            stream.Seek(archiveOffset + tableOffset, SeekOrigin.Begin);
            var result = await sr.ReadBlockTableAsync(stream, archiveHeader.BlockTableCount, md5);
            return result;
        }

        private async Task<ushort[]?> ReadHiBlockTableAsync(
            Stream stream,
            long archiveOffset,
            MpqStreamReader sr,
            MpqArchiveHeader archiveHeader)
        {
            if (archiveHeader is MpqArchiveHeaderV3 archiveHeader3)
            {
                long tableOffset = archiveHeader3.HiBlockTableOffset;
                if (tableOffset == 0)
                    return null;

                var md5 = (archiveHeader as MpqArchiveHeaderV4)?.Md5HiBlockTable;

                stream.Seek(archiveOffset + tableOffset, SeekOrigin.Begin);
                var result = await sr.ReadHiBlockTableAsync(stream, archiveHeader.BlockTableCount, md5);

                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
