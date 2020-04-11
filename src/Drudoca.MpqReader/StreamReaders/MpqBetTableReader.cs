using System;
using System.IO;
using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqBetTableReader
    {
        private IMd5Validation _md5Validation;
        private IEncryption _encryption;

        public MpqBetTableReader(IMd5Validation md5Validation, IEncryption encryption)
        {
            _md5Validation = md5Validation;
            _encryption = encryption;
        }

        public Task<MpqBetTable> ReadAsync(Stream stream, byte[]? md5)
        {
            throw new NotImplementedException();
        }
    }
}
