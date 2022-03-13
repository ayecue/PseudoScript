using Microsoft.VisualStudio.TestTools.UnitTesting;
using PseudoScript;
using PseudoScript.Interpreter;
using PseudoScript.Interpreter.CustomTypes;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyMSTest;

namespace IntrinsicsTests
{
    [TestClass()]
    public class IntrinsicsTests : VerifyBase
    {
        Task RunFixture(string name)
        {
            string fixturePath = PseudoScriptTests.Utils.GetFixturePath(name);

            Dictionary<string, CustomValue> mockedApiInterface = new();
            List<string> output = new();
            Interpreter interpreter = null;

            mockedApiInterface.Add(
                "print",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("message", out CustomValue value)) output.Add(value.ToString());
                    return CustomNil.Void;
                })
                    .AddArgument("message")
            );

            Dictionary<string, CustomValue> apiInterface = Intrinsics.Init(mockedApiInterface);

            Options options = new(fixturePath, apiInterface, null, null, null);
            interpreter = new(options);
            interpreter.Run().Wait();

            return Verify(output);
        }

        [TestMethod()]
        public Task RunTestWithSimpleObject()
        {
            return RunFixture("intrinsics-test.src");
        }
    }
}