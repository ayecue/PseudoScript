using System;

namespace PseudoScript.Lexer
{
    public class Token
    {
        public static class Type
        {
            public const string EOF = "EOF";
            public const string StringLiteral = "StringLiteral";
            public const string Keyword = "Keyword";
            public const string Identifier = "Identifier";
            public const string NumericLiteral = "NumericLiteral";
            public const string Punctuator = "Punctuator";
            public const string BooleanLiteral = "BooleanLiteral";
            public const string NilLiteral = "NilLiteral";
            public const string EOL = "EOL";
        }

        public readonly string type;
        public readonly string value;

        public readonly int line;
        public readonly int lineStart;

        public readonly Range range;
        public readonly Range lineRange;

        public readonly int? lastLine;
        public readonly int? lastLineStart;

        public Token(
            string type,
            string value,
            int line,
            int lineStart,
            Range range,
            int offset,
            int? lastLine,
            int? lastLineStart
        )
        {
            this.type = type;
            this.value = value;
            this.line = line;
            this.lineStart = lineStart;
            this.range = range;
            this.lineRange = new Range(range.Start.Value - offset + 1, range.End.Value - offset + 1);
            this.lastLine = lastLine;
            this.lastLineStart = lastLineStart;
        }

        public Token(
            string type,
            string value,
            int line,
            int lineStart,
            Range range,
            int offset
        ) : this(type, value, line, lineStart, range, offset, null, null) { }
    }
}
