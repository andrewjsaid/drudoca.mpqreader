using System.IO;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader
{
    public class MpqArchive
    {
        private readonly Stream _stream;
        private readonly int _userDataHeaderOffset;
        private readonly MpqUserDataHeader _userDataHeader;
        private readonly int _archiveHeaderOffset;
        private readonly MpqArchiveHeader _archiveHeader;
        private readonly MpqHashTable[] _hashTable;
        private readonly MpqBlockTable[] _blockTable;
        private readonly ushort[]? _hiBlockTable;

        internal MpqArchive(
            Stream stream,
            int userDataHeaderOffset,
            MpqUserDataHeader userDataHeader,
            int archiveHeaderOffset,
            MpqArchiveHeader archiveHeader,
            MpqHashTable[] hashTable,
            MpqBlockTable[] blockTable,
            ushort[]? hiBlockTable)
        {
            _stream = stream;
            _userDataHeaderOffset = userDataHeaderOffset;
            _userDataHeader = userDataHeader;
            _archiveHeaderOffset = archiveHeaderOffset;
            _archiveHeader = archiveHeader;
            _hashTable = hashTable;
            _blockTable = blockTable;
            _hiBlockTable = hiBlockTable;
        }

    }
}
