using System;
using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader
{
    public class MpqArchive : IDisposable, IAsyncDisposable
    {
        private readonly Stream _stream;
        private readonly bool _disposeStream;
        private readonly long _userDataHeaderOffset;
        private readonly MpqUserDataHeader _userDataHeader;
        private readonly long _archiveHeaderOffset;
        private readonly MpqArchiveHeader _archiveHeader;
        private readonly BasicFileTable? _basicFileTable;
        private readonly ExFileTable? _exFileTable;

        internal MpqArchive(
            Stream stream,
            bool disposeStream,
            long userDataHeaderOffset,
            MpqUserDataHeader userDataHeader,
            long archiveHeaderOffset,
            MpqArchiveHeader archiveHeader,
            BasicFileTable? basicFileTable,
            ExFileTable? exFileTable)
        {
            _stream = stream;
            _disposeStream = disposeStream;
            _userDataHeaderOffset = userDataHeaderOffset;
            _userDataHeader = userDataHeader;
            _archiveHeaderOffset = archiveHeaderOffset;
            _archiveHeader = archiveHeader;
            _basicFileTable = basicFileTable;
            _exFileTable = exFileTable;
        }

        public void Dispose()
        {
            if (_disposeStream)
            {
                _stream.Dispose();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposeStream)
            {
                await _stream.DisposeAsync();
            }
        }
    }
}
