using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace S_Coefficient.Tests
{
    [TestClass]
    public class DataReaderTests
    {
        [TestMethod]
        public void TestReadRAD()
        {
            var actual = DataReader.ReadRAD("Ca-45");
            var expected = File.ReadAllLines(TestFiles.Combine("ReadRadExpected.txt"));

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestReadBET()
        {
            var actual = DataReader.ReadBET("Sr-90");
            var expected = File.ReadAllLines(TestFiles.Combine("ReadBetExpected.txt"));

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
