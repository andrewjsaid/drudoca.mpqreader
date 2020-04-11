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
            var sr = new MpqStreamReader();

            long userDataHeaderOffset = 0;

            var userDataHeader = await sr.ReadUserDataHeaderAsync(stream);
            if (userDataHeader == null)
            {
                // TODO: Search for user header, or allow file to start without user data.
                throw new NotImplementedException("File must start with user data header.");
            }

            var archiveOffset = userDataHeaderOffset + userDataHeader.HeaderOffset;
            stream.Seek(archiveOffset, SeekOrigin.Begin);
            var archiveHeader = await sr.ReadArchiveHeaderAsync(stream);
            if (archiveHeader == null)
            {
                // TODO: Later we must support files without UserDataHeader telling us where this one is.
                throw new InvalidDataException("FileHeader not found.");
            }

            var hashTable = await ReadHashTableAsync(stream, archiveOffset, sr, archiveHeader);
            var blockTable = await ReadBlockTableAsync(stream, archiveOffset, sr, archiveHeader);

            return null!;
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
            stream.Seek(archiveOffset + tableOffset, SeekOrigin.Begin);
            var result = await sr.ReadHashTableAsync(stream, archiveHeader.HashTableCount);
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
            stream.Seek(archiveOffset + tableOffset, SeekOrigin.Begin);
            var result = await sr.ReadBlockTableAsync(stream, archiveHeader.BlockTableCount);
            return result;
        }

    }

    public class MpqArchive
    {

    }
}
