using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.UnitTests.Helpers
{
    public class TestFilesHelper
    {
        private static Assembly assembly = Assembly.GetExecutingAssembly();

        public static byte[] GetTestFile(string name)
        {
            using (var stream = assembly.GetManifestResourceStream($"CognitivePipeline.Functions.UnitTests.TestFiles.{name}"))
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
