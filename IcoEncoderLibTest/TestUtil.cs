using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IcoEncoderLibTest
{
    public static class TestUtil
    {
        public static string GetResourceFilePath(string fileName)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", fileName);
        }

        public static string GetTestFilePath(string fileName, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerMember = "")
        {
            string testOutputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestOutput");
            string testCaseDirecotry = Path.Combine(testOutputDirectory, Path.GetFileNameWithoutExtension(callerFile), callerMember);
            Directory.CreateDirectory(testCaseDirecotry);
            return Path.Combine(testCaseDirecotry, fileName);
        }
    }
}
