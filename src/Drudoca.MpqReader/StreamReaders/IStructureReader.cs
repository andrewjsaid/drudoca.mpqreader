using System.Threading.Tasks;

namespace Drudoca.MpqReader.StreamReaders
{
    internal interface IStructureReader<T>
    {
        int InitialSize { get; }
        ValueTask<T> ReadAsync(MpqStreamReaderContext context);
    }
}
