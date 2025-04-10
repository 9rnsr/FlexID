using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlexID.Calc.Tests
{
    [TestClass]
    public class IndexReadTests
    {
        [TestMethod]
        public void TestReadNDX()
        {
            foreach (var actual in IndexDataReader.ReadNDX())
            {
            }
        }
    }
}
