using System;
using System.IO;
using System.Threading.Tasks;

namespace Drudoca.MpqReader
{
    public class MpqReader
    {

        public async Task<MpqArchive> ReadAsync(Stream stream)
        {
            var sr = new MpqStreamReader(stream);

            int userDataHeaderOffset = 0;

            var userDataHeader = await sr.ReadUserDataHeaderAsync();
            if (userDataHeader == null)
            {
                // TODO: Search for user header, or allow file to start without user data.
                throw new NotImplementedException("File must start with user data header.");
            }

            var fileHeaderOffset = userDataHeaderOffset + userDataHeader.HeaderOffset;
            stream.Seek(fileHeaderOffset, SeekOrigin.Begin);
            var fileHeader = await sr.ReadFileHeaderAsync();

            return null!;
        }

    }

    public class MpqArchive
    {

    }
}
