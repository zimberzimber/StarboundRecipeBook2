using Jil;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading;

namespace SBRB_Testing
{
    [TestClass]
    public class UnitTest1
    {
        string testSubjectDirectory;

        UnitTest1()
        {
            testSubjectDirectory = Directory.GetCurrentDirectory();
        }

        [TestMethod]
        public void RemovingComments()
        {
            Thread.Sleep(1000000);
            var x = temp(5, 1);
        }

        int temp(int x, int y) => x + y;
    }
}
