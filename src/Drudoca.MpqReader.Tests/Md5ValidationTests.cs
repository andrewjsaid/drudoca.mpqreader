using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drudoca.MpqReader.Tests
{
    [TestClass]
    public class Md5ValidationTests
    {

        [TestMethod]
        public void TestEqual()
        {
            const string dataString = "asdf";
            const string hashString = "912ec803b2ce49e4a541068d495ab570";
            var result = Compare(dataString, hashString);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestSlightlyUnEqual1()
        {
            const string dataString = "asdf";
            const string hashString = "912ec803b2cf49e4a541068d495ab570";
            var result = Compare(dataString, hashString);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestSlightlyUnEqual2()
        {
            const string dataString = "asdf";
            const string hashString = "912ec803b2ce49e4a541068d495ab571";
            var result = Compare(dataString, hashString);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestCompletelyEqual()
        {
            const string dataString = "asdd";
            const string hashString = "912ec803b2ce49e4a541068d495ab570";
            var result = Compare(dataString, hashString);
            Assert.IsFalse(result);
        }

        private bool Compare(string dataString, string hashString)
        {
            var data = Encoding.ASCII.GetBytes(dataString);

            var hash = new List<byte>();
            for (var i = 0; i < hashString.Length; i += 2)
            {
                var b = int.Parse(hashString[i..(i + 2)], System.Globalization.NumberStyles.HexNumber);
                hash.Add((byte)b);
            }

            var result = Md5Validation.IsValid(data, 0, data.Length, hash.ToArray());
            return result;
        }

    }
}
