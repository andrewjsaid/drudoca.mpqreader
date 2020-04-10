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
            var file = File.OpenRead("files/g1.SC2REPLAY");
            var reader = new MpqReader();
            await reader.ReadAsync(file);
        }
    }
}
