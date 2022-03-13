using Microsoft.VisualStudio.TestTools.UnitTesting;
using PseudoScript.Lexer;
using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyMSTest;

namespace LexerTests
{
    [TestClass()]
    public class LexerTests : VerifyBase
    {
        Task RunFixture(string name)
        {
            string content = PseudoScriptTests.Utils.LoadFixture(name);

            Lexer lexer = new(content);
            List<Token> tokens = new();
            Token token = lexer.Next();

            while (token.value != "<eof>")
            {
                tokens.Add(token);
                token = lexer.Next();
            }

            return Verify(tokens);
        }

        [TestMethod()]
        public Task LexerTestWithSimpleObject()
        {
            return RunFixture("simple-object.src");
        }

        [TestMethod()]
        public Task LexerTestWithBinaryExpressions()
        {
            return RunFixture("binary-expressions.src");
        }

        [TestMethod()]
        public Task LexerTestWithEol()
        {
            return RunFixture("eol.src");
        }

        [TestMethod()]
        public Task LexerTestWithIfClause()
        {
            return RunFixture("if-clause.src");
        }

        [TestMethod()]
        public Task LexerTestWithIfShorthand()
        {
            return RunFixture("if-shorthand.src");
        }

        [TestMethod()]
        public Task LexerTestWithNegation()
        {
            return RunFixture("negation.src");
        }

        [TestMethod()]
        public Task LexerTestWithObjInit()
        {
            return RunFixture("obj-init.src");
        }

        [TestMethod()]
        public Task LexerTestWithUnary()
        {
            return RunFixture("unary.src");
        }
    }
}