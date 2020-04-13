namespace Drudoca.MpqReader
{
    internal class ExFileTableSearch
    {
        private readonly ICrypto _crypto;

        public ExFileTableSearch(
            ICrypto crypto)
        {
            _crypto = crypto;
        }

        public IFileTableEntry? Search(ExFileTable table, string path)
        {
            return null;
        }

    }
}
