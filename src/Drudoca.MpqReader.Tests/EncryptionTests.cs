using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drudoca.MpqReader.Tests
{
    [TestClass]
    public class EncryptionTests
    {

        [TestMethod]
        public void TestHto1()
        {
            const string text = "unit\\neutral\\acritter.grp";
            const ulong expected = 0xA26067F3;

            var sut = new Encryption();
            var result = sut.HashTableOffset(text);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestHto2()
        {
            const string text = "arr/units.dat";
            const ulong expected = 0xF4E6C69D;

            var sut = new Encryption();
            var result = sut.HashTableOffset(text);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestFileKey1()
        {
            const string text = "(hash table)";
            const ulong expected = 0xc3af3770;

            var sut = new Encryption();
            var result = sut.HashFileKey(text);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestFileKey2()
        {
            const string text = "(Block Table)";
            const ulong expected = 0xec83b3a3;

            var sut = new Encryption();
            var result = sut.HashFileKey(text);

            Assert.AreEqual(expected, result);
        }

    }
}
