using System;
using System.IO;

namespace PseudoScriptTests
{
    static class Utils
    {
        static readonly string directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        public static string GetFixturePath(string name)
        {
            return System.IO.Path.Combine(directory, "../../../Fixtures/" + name);
        }

        public static string LoadFixture(string name)
        {
            string path = GetFixturePath(name);

            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            throw new Exception(string.Format("Fixture {0} does not exist.", name));
        }
    }
}
