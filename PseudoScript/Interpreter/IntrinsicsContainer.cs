using PseudoScript.Interpreter.CustomTypes;
using System.Collections.Generic;

namespace PseudoScript.Interpreter
{
    public class IntrinsicsContainer
    {
        Dictionary<string, CustomFunction> intrinsics = new();

        public IntrinsicsContainer Add(string name, CustomFunction fn)
        {
            intrinsics[name] = fn;
            return this;
        }

        public bool Has(string name)
        {
            return intrinsics.ContainsKey(name);
        }

        public CustomFunction Get(string name)
        {
            intrinsics.TryGetValue(name, out CustomFunction intrinsic);
            return intrinsic;
        }
    }
}
