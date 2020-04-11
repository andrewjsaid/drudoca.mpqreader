using System;
using System.Threading.Tasks;
using Drudoca.MpqReader.Structures;

namespace Drudoca.MpqReader.StreamReaders
{
    internal class MpqBetTableReader : IStructureReader<MpqBetTable>
    {
        public int InitialSize => throw new NotImplementedException();

        public ValueTask<MpqBetTable> ReadAsync(MpqStreamReaderContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
