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
        class CustomOutputHandler : OutputHandler
        {
            public List<string> output = new();

            public override void Print(string message)
            {
                output.Add(message);
            }
        }

        Task RunFixture(string name)
        {
            string fixturePath = PseudoScriptTests.Utils.GetFixturePath(name);

            Dictionary<string, CustomValue> mockedApiInterface = new();

            Dictionary<string, CustomValue> apiInterface = Intrinsics.Init(mockedApiInterface);
            Interpreter interpreter = new(fixturePath, apiInterface);
            CustomOutputHandler outputHandler = new();

            interpreter.SetOutputHandler(outputHandler);

            interpreter.Run().Wait();

            return Verify(outputHandler.output);
        }

        [TestMethod()]
        public Task RunTestWithIntrinsics()
        {
            return RunFixture("intrinsics-test.src");
        }
    }
}