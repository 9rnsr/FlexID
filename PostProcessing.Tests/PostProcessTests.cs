using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace PostProcessing.Tests
{
    [TestClass]
    public class PostProcessTests
    {
        [TestMethod]
        public void Test()
        {
            var sourceFile = TestFiles.Combine("PostProcessing", "source.txt");
            var sourceLines = File.ReadAllLines(sourceFile);

            var resultLines = PostProcessing.Program.FormatFile(sourceLines).ToArray();

            // 目視確認用。
            //var resultFile = TestFiles.Combine("PostProcessing", "~result.txt");
            //File.WriteAllLines(resultFile, resultLines);

            var expectFile = TestFiles.Combine("PostProcessing", "expect.txt");
            var expectLines = File.ReadAllLines(expectFile).ToArray();
            CollectionAssert.AreEqual(expectLines, resultLines);
        }
    }
}
