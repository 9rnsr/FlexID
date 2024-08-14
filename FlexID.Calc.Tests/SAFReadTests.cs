using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace FlexID.Calc.Tests
{
    [TestClass]
    public class SAFReadTests
    {
        [TestMethod]
        public void TestReadRAD()
        {
            var expectFile = TestFiles.Combine("SAFRead", "Ca-45_RadExpected.txt");
            var expectLines = File.ReadAllLines(expectFile);

            var actualLines = SAFDataReader.ReadRAD("Ca-45");

            CollectionAssert.AreEqual(expectLines, actualLines);
        }

        [TestMethod]
        public void TestReadBET()
        {
            var expectFile = TestFiles.Combine("SAFRead", "Sr-90_BetExpected.txt");
            var expectLines = File.ReadAllLines(expectFile);

            var actualLines = SAFDataReader.ReadBET("Sr-90");

            CollectionAssert.AreEqual(expectLines, actualLines);
        }

        [TestMethod]
        public void TestReadSAF_AdultMale()
        {
            var safdata = SAFDataReader.ReadSAF(Sex.Male);
            Assert.IsNotNull(safdata);
        }

        [TestMethod]
        public void TestReadSAF_AdultFemale()
        {
            var safdata = SAFDataReader.ReadSAF(Sex.Female);
            Assert.IsNotNull(safdata);
        }
    }
}
