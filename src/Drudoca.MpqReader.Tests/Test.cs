using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drudoca.MpqReader.Tests
{
    [TestClass]
    public class Test
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            const string path = @"files\g1.SC2REPLAY";
            using var file = File.OpenRead(path);
            var reader = new MpqReader();
            await reader.ReadAsync(file);
        }
    }
}
