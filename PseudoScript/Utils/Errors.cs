using PseudoScript.Parser;
using System;

namespace PseudoScript
{
    public class PseudoScriptException : Exception
    {
        public readonly string prefix;
        public readonly int line;

        public PseudoScriptException(string prefix, string message) : this(prefix, message, -1) { }
        public PseudoScriptException(string prefix, string message, int line) : base(string.Format("{0}: {1}", prefix, message))
        {
            this.prefix = prefix;
            this.line = line;
        }
    }

    public class LexerException : PseudoScriptException
    {
        readonly Lexer.Token token;

        public LexerException(string message, int line) : base("LexerException", message, line) { }

        public LexerException(string message, Lexer.Token token) : base("LexerException", message, token.line)
        {
            this.token = token;
        }
    }

    internal class UnexpectedStringEOL : LexerException
    {
        internal UnexpectedStringEOL(int line) : base(string.Format("Unexpected string ending at line {0}.", line), line) { }
    }

    internal class InvalidCharacter : LexerException
    {
        internal readonly CharacterCode code;

        internal InvalidCharacter(CharacterCode code, int line) : base(string.Format("Invalid character {0} at line {1}.", code, line), line)
        {
            this.code = code;
        }
    }

    public class ParserException : PseudoScriptException
    {
        readonly AstProvider.Position position;

        public ParserException(string message, AstProvider.Position position) : base("ParserException", message, position.line)
        {
            this.position = position;
        }

        public ParserException(string message, Lexer.Token token) : base("ParserException", message, token.line)
        {
            this.position = new AstProvider.Position(token.line, token.lineRange.Start.Value);
        }
    }

    internal class UnexpectedValue : ParserException
    {
        internal readonly string expected;

        internal UnexpectedValue(Lexer.Token token, string expected) : base(string.Format("Unexpected value {0} at line {1}. Expected: {2}.", token.value, token.line, expected), token)
        {
            this.expected = expected;
        }
    }

    internal class UnexpectedValues : ParserException
    {
        internal readonly string[] expectedList;

        internal UnexpectedValues(Lexer.Token token, string[] expectedList) : base(string.Format("Unexpected value {0} at line {1}. Expected one of: {2}.", token.value, token.line, String.Join(", ", expectedList)), token)
        {
            this.expectedList = expectedList;
        }
    }

    internal class UnexpectedIdentifier : ParserException
    {
        internal UnexpectedIdentifier(Lexer.Token token) : base(string.Format("Unexpected identifier {0} at line {1}.", token.value, token.line), token) { }
    }

    internal class UnexpectedEndOfIfStatement : ParserException
    {
        internal UnexpectedEndOfIfStatement(Lexer.Token token) : base(string.Format("Unexpected end of if statement {0} at line {1}.", token.value, token.line), token) { }
    }

    internal class UnexpectedArguments : ParserException
    {
        internal readonly Parser.AstProvider.Base origin;

        internal UnexpectedArguments(Lexer.Token token, Parser.AstProvider.Base origin) : base(string.Format("{0} received unexpected arguments {1} at line {2}.", origin.type, token.value, token.line), token)
        {
            this.origin = origin;
        }
    }

    internal class UnexpectedAssignmentOrCall : ParserException
    {
        internal UnexpectedAssignmentOrCall(Lexer.Token token) : base(string.Format("Unexpected assignment or call at line {0}.", token.line), token) { }
    }

    internal class UnexpectedExpression : ParserException
    {
        internal UnexpectedExpression(Lexer.Token token) : base(string.Format("Unexpected expression at line {0}.", token.line), token) { }
    }

    internal class UnexpectedParameterInFunction : ParserException
    {
        internal UnexpectedParameterInFunction(Lexer.Token token) : base(string.Format("Unexpected parameter in function declaration at line {0}.", token.line), token) { }
    }

    internal class UnexpectedEOF : ParserException
    {
        internal UnexpectedEOF(Lexer.Token token) : base(string.Format("Unexpected end of file at line {0}.", token.line), token) { }
    }

    internal class UnexpectedImport : ParserException
    {
        internal UnexpectedImport(Lexer.Token token) : base(string.Format("Unexpected import path at {0}.", token.line), token) { }
    }

    internal class CallExpressionEOL : ParserException
    {
        readonly Lexer.Token previousToken;

        internal CallExpressionEOL(Lexer.Token token, Lexer.Token previousToken) : base(string.Format("Call expressions do not support multiline arguments. Discrepancy found between {0} and {1}.", previousToken.line, token.line), token)
        {
            this.previousToken = previousToken;
        }
    }

    public class InterpreterException : PseudoScriptException
    {
        readonly AstProvider.Base item;

        public InterpreterException(string message) : base("InterpreterException", message) { }

        public InterpreterException(string message, AstProvider.Base item) : base("InterpreterException", message, item.start.line)
        {
            this.item = item;
        }
    }
}
