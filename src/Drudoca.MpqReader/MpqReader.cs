using System;
using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.StreamReaders;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader
{
    public class MpqReader
    {

        public async Task<MpqArchive> ReadAsync(Stream stream)
        {
            var sr = new MpqStreamReader(new Md5Validation(), new Encryption());

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

            // TODO: Verify digital signature.

            return null!;
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

                var md5 = (archiveHeader as MpqArchiveHeaderV4)?.Md5HetTable;
                // TODO AJS: HET Table Compression is not implemented

                stream.Seek(archiveOffset + tableOffset, SeekOrigin.Begin);
                var result = await sr.ReadHetTableAsync(stream, md5);

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

                var md5 = (archiveHeader as MpqArchiveHeaderV4)?.Md5BetTable;
                // TODO AJS: HET Table Compression is not implemented

                stream.Seek(archiveOffset + tableOffset, SeekOrigin.Begin);
                var result = await sr.ReadBetTableAsync(stream, md5);

                return result;
            }
            else
            {
                return null;
            }
        }

        private async Task<MpqHashTable[]> ReadHashTableAsync(
            Stream stream,
            long archiveOffset,
            MpqStreamReader sr,
            MpqArchiveHeader archiveHeader)
        {
            long tableOffset = archiveHeader.HashTableOffset;

            if (archiveHeader is MpqArchiveHeaderV2 archiveHeader2)
            {
                tableOffset = ((long)archiveHeader2.HashTableOffsetHi << 32) | tableOffset;
            }

            var md5 = (archiveHeader as MpqArchiveHeaderV4)?.Md5HashTable;

            stream.Seek(archiveOffset + tableOffset, SeekOrigin.Begin);
            var result = await sr.ReadHashTableAsync(stream, archiveHeader.HashTableCount, md5);
            return result;
        }

        private async Task<MpqBlockTable[]> ReadBlockTableAsync(
            Stream stream,
            long archiveOffset,
            MpqStreamReader sr,
            MpqArchiveHeader archiveHeader)
        {
            long tableOffset = archiveHeader.BlockTableOffset;

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
