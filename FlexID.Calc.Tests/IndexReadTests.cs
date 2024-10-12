using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlexID.Calc.Tests
{
    [TestClass]
    public class IndexReadTests
    {
        [TestMethod]
        public void TestReadNDX()
        {
            //var expectFile = TestFiles.Combine("SAFRead", "Ca-45_RadExpected.txt");
            //var expectLines = File.ReadAllLines(expectFile);

            /*var actuals = */
            IndexDataReader.ReadNDX();
            //foreach (var actual in actuals)
            //{ }

            //CollectionAssert.AreEqual(expectLines, actualLines);

        }
    }
}
