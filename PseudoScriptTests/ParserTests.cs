using Microsoft.VisualStudio.TestTools.UnitTesting;
using PseudoScript.Parser;
using System.Threading.Tasks;
using VerifyMSTest;

namespace ParserTests
{
    [TestClass()]
    public class ParserTests : VerifyBase
    {
        Task RunFixture(string name)
        {
            string content = PseudoScriptTests.Utils.LoadFixture(name);

            Parser parser = new(content);
            AstProvider.Base chunk = parser.ParseChunk();

            return Verify(chunk);
        }

        [TestMethod()]
        public Task ParserTestWithSimpleObject()
        {
            return RunFixture("simple-object.src");
        }

        [TestMethod()]
        public Task ParserTestWithBinaryExpressions()
        {
            return RunFixture("binary-expressions.src");
        }

        [TestMethod()]
        public Task ParserTestWithEol()
        {
            return RunFixture("eol.src");
        }

        [TestMethod()]
        public Task ParserTestWithIfClause()
        {
            return RunFixture("if-clause.src");
        }

        [TestMethod()]
        public Task ParserTestWithIfShorthand()
        {
            return RunFixture("if-shorthand.src");
        }

        [TestMethod()]
        public Task ParserTestWithNegation()
        {
            return RunFixture("negation.src");
        }

        [TestMethod()]
        public Task ParserTestWithObjInit()
        {
            return RunFixture("obj-init.src");
        }

        [TestMethod()]
        public Task ParserTestWithUnary()
        {
            return RunFixture("unary.src");
        }
    }
}