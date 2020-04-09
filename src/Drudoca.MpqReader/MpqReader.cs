using System.IO;
using System.Threading.Tasks;

namespace Drudoca.MpqReader
{
    public class MpqReader
    {

        public async Task ReadAsync(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            using var br = new BinaryReader(ms);

            // 0x1A51504D
            var num = 0x1B51504D;

            var x = br.ReadInt32();

        }

    }
}
