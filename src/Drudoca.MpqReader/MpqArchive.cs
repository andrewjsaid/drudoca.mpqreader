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

        internal ICrypto? Crypto { get; set; }

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

        public MpqArchiveFile? GetFile(string path)
        {
            IFileTableEntry? fte = null;

            Crypto ??= new Crypto();

            if (_basicFileTable != null)
            {
                var searcher = new BasicFileTableSearch(Crypto);
                fte = searcher.Search(_basicFileTable, path);
            }

            if (_exFileTable != null && fte == null)
            {
                var searcher = new ExFileTableSearch(Crypto);
                fte = searcher.Search(_exFileTable, path);
            }

            if (fte == null || (fte.Flags & BlockFileFlags.Exists) == 0)
            {
                // File not found
                return null;
            }

            var result = new MpqArchiveFile(this, fte);
            return result;
        }

        internal int BlockSize => _archiveHeader.BlockSize;
        internal void Seek(long archiveOffset) => _stream.Seek(_archiveHeaderOffset + archiveOffset, SeekOrigin.Begin);
        internal ValueTask<int> ReadAsync(Memory<byte> memory) => _stream.ReadAsync(memory);

    }
}
