namespace Drudoca.MpqReader
{
    internal class BasicFileTableSearch
    {
        private readonly ICrypto _crypto;

        public BasicFileTableSearch(
            ICrypto crypto)
        {
            _crypto = crypto;
        }

        public IFileTableEntry? Search(BasicFileTable table, string path)
        {
            // This algorithm should probably use the locale somehow...

            var tableIndex = (int)_crypto.HashTableIndex(path) % table.Length;

            var nameHash1 = _crypto.HashName1(path);
            var nameHash2 = _crypto.HashName2(path);

            var i = tableIndex;
            do
            {
                var entry = table.Entries[i];
                if(entry == null)
                {
                    // File should be here if it exists
                    break;
                }

                if(entry.NameHash1 == nameHash1 && entry.NameHash2 == nameHash2)
                {
                    // We found the file
                    if (entry.IsDeleted)
                    {
                        // ... but it no longer exists
                        break;
                    }

                    return entry;
                }

                i = (i + 1) % table.Length;
            } while (i != tableIndex);

            return null;
        }

    }
}
