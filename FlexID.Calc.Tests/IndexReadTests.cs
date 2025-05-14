using Xunit;

namespace FlexID.Calc.Tests
{
    public class IndexReadTests
    {
        [Fact]
        public void TestReadNDX()
        {
            foreach (var actual in IndexDataReader.ReadNDX())
            {
            }
        }
    }
}
