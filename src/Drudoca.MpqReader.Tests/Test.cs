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

            var reader = new MpqReader();

            using var archive = await reader.ReadAsync(File.OpenRead(path), true);

            Assert.IsNotNull(archive);

            var fileNames = await archive.ReadFileListAsync();
            Assert.IsNotNull(fileNames);

            foreach (var fileName in fileNames!)
            {
                var file = archive.GetFile(fileName)!;
                Assert.IsNotNull(file);
                var fileData = await file.ReadAsync();
            }

        }
    }
}
