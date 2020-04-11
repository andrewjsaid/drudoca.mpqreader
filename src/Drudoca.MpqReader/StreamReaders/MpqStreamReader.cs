using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    /// <summary>
    /// Facade
    /// </summary>
    internal sealed class MpqStreamReader
    {

        private readonly IMd5Validation _md5Validation;
        private readonly IEncryption _encryption;

        public MpqStreamReader(
            IMd5Validation md5Validation,
            IEncryption encryption)
        {
            _md5Validation = md5Validation;
            _encryption = encryption;
        }


        public Task<MpqArchiveHeader?> ReadArchiveHeaderAsync(Stream stream)
            => new MpqArchiveHeaderReader(_md5Validation).ReadAsync(stream);

        public Task<MpqUserDataHeader?> ReadUserDataHeaderAsync(Stream stream)
            => new MpqUserDataHeaderReader().ReadAsync(stream);

        public Task<MpqHetTable?> ReadHetTableAsync(Stream stream, byte[]? md5)
            => new MpqHetTableReader(_md5Validation, _encryption).ReadAsync(stream, md5);

        public Task<MpqBetTable> ReadBetTableAsync(Stream stream, byte[]? md5)
            => new MpqBetTableReader(_md5Validation, _encryption).ReadAsync(stream, md5);

        public Task<MpqHashTable[]> ReadHashTableAsync(Stream stream, int count, byte[]? md5)
            => new MpqHashTableReader(_md5Validation, _encryption).ReadAsync(stream, count, md5);

        public Task<MpqBlockTable[]> ReadBlockTableAsync(Stream stream, int count, byte[]? md5)
            => new MpqBlockTableReader(_md5Validation, _encryption).ReadAsync(stream, count, md5);

        public Task<ushort[]> ReadHiBlockTableAsync(Stream stream, int count, byte[]? md5)
            => new MpqHiBlockTableReader(_md5Validation).ReadAsync(stream, count, md5);

    }
}
