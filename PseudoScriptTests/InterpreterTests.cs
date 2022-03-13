using Microsoft.VisualStudio.TestTools.UnitTesting;
using PseudoScript.Interpreter;
using PseudoScript.Interpreter.CustomTypes;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyMSTest;

namespace InterpreterTests
{
    [TestClass()]
    public class InterpreterTests : VerifyBase
    {
        Task RunFixture(string name)
        {
            string fixturePath = PseudoScriptTests.Utils.GetFixturePath(name);

            Dictionary<string, CustomValue> apiInterface = new();
            List<string> output = new();
            Interpreter interpreter = null;

            apiInterface.Add(
                "print",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("message", out CustomValue value)) output.Add(value.ToString());
                    return CustomNil.Void;
                })
                    .AddArgument("message")
            );

            apiInterface.Add(
                "typeof",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("object", out CustomValue value)) return new CustomString(value.GetCustomType());
                    return CustomNil.Void;
                })
                    .AddArgument("object")
            );

            apiInterface.Add(
                "exit",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("message", out CustomValue value)) output.Add(value.ToString());
                    interpreter?.Exit();
                    return CustomNil.Void;
                })
                    .AddArgument("message")
            );

            apiInterface.Add(
                "range",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    CustomValue to = arguments["to"];
                    CustomValue from = arguments["from"];
                    CustomValue step = arguments["step"];

                    if (to == CustomNil.Void)
                    {
                        to = from;
                        from = new CustomNumber(0);
                    }

                    CustomList list = new();

                    for (int index = from.ToInt(); index < to.ToInt(); index += step.ToInt())
                    {
                        list.value.Add(new CustomNumber(index));
                    }

                    return list;
                })
                    .AddArgument("from")
                    .AddArgument("to")
                    .AddArgument("step", new CustomNumber(1))
            );

            apiInterface.Add(
                "get_test_interface",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    CustomInterface itr = new("testInterface");

                    itr.AddFunction("test", new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                    {
                        if (arguments.TryGetValue("message", out CustomValue value)) output.Add("test interface print: " + value.ToString());
                        return CustomNil.Void;
                    })
                        .AddArgument("message")
                    );

                    return itr;
                })
            );

            CustomList.AddIntrinsic("len", new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
            {
                CustomList list = (CustomList)self;
                return new CustomNumber(list.value.Count);
            }));

            Options options = new(fixturePath, apiInterface, null, null, null);
            interpreter = new(options);
            interpreter.Run().Wait();

            return Verify(output);
        }

        [TestMethod()]
        public Task RunTestWithSimpleObject()
        {
            return RunFixture("simple-object.src");
        }

        [TestMethod()]
        public Task RunTestWithIfClause()
        {
            return RunFixture("if-clause.src");
        }

        [TestMethod()]
        public Task RunTestWithExitInLoop()
        {
            return RunFixture("exit-in-loop.src");
        }

        [TestMethod()]
        public Task RunTestWithDefaultArgs()
        {
            return RunFixture("default-args.src");
        }

        [TestMethod()]
        public Task RunTestWithManipulation()
        {
            return RunFixture("manipulation.src");
        }

        [TestMethod()]
        public Task RunTestWithFunction()
        {
            return RunFixture("function.src");
        }

        [TestMethod()]
        public Task RunTestWithImport()
        {
            return RunFixture("import.src");
        }

        [TestMethod()]
        public Task RunTestWithForRange()
        {
            return RunFixture("for-range.src");
        }

        [TestMethod()]
        public Task RunTestWithFloat()
        {
            return RunFixture("float.src");
        }

        [TestMethod()]
        public Task RunTestWithTestInterface()
        {
            return RunFixture("interface-test.src");
        }

        [TestMethod()]
        public Task RunTestWithEol()
        {
            return RunFixture("eol.src");
        }

        [TestMethod()]
        public Task RunTestWithNegation()
        {
            return RunFixture("negation.src");
        }

        [TestMethod()]
        public Task RunTestWithInstances()
        {
            return RunFixture("instances.src");
        }
    }
}