using System.IO;

namespace PseudoScript.Interpreter.Handler
{
    abstract public class Resource
    {
        abstract public string GetTargetRelativeTo(string source, string target);
        abstract public bool Has(string target);
        abstract public string Get(string target);
        abstract public string Resolve(string target);
    }

    internal class DefaultResource : Resource
    {
        public override string Get(string target)
        {
            if (File.Exists(target))
            {
                return File.ReadAllText(target);
            }

            return null;
        }

        public override string GetTargetRelativeTo(string source, string target)
        {
            string origin = Path.GetFullPath(Path.Combine(source, ".."));
            string result = Path.GetFullPath(Path.Combine(origin, target));

            if (File.Exists(result))
            {
                return result;
            }

            return null;
        }

        public override bool Has(string target)
        {
            return File.Exists(target);
        }

        public override string Resolve(string target)
        {
            return Path.GetFullPath(target);
        }
    }
}
