using System;
using System.Linq;

namespace PseudoScript.Parser
{
    public class Validator
    {
        private static readonly string[] breakingBlockShortcutKeywords = {
            "if",
            "else",
            "else if",
            "<eof>",
            "end for",
            "end while",
            "end function",
            "end if"
        };

        private static readonly string[] nonNilLiterals = {
            Lexer.Token.Type.StringLiteral,
            Lexer.Token.Type.NumericLiteral,
            Lexer.Token.Type.BooleanLiteral
        };

        private static readonly string[] literals = {
            Lexer.Token.Type.StringLiteral,
            Lexer.Token.Type.NumericLiteral,
            Lexer.Token.Type.BooleanLiteral,
            Lexer.Token.Type.NilLiteral
        };

        private static readonly string[] expressionOperators = {
                Operator.Plus,
                Operator.Asterik,
                Operator.Minus,
                Operator.Slash,
                Operator.PercentSign,
                Operator.LessThan,
                Operator.GreaterThan,
                Operator.LessThanOrEqual,
                Operator.GreaterThanOrEqual,
                Operator.NotEqual,
                Operator.Equal,
                Operator.Or,
                Operator.And,
                Operator.BitwiseAnd,
                Operator.BitwiseOr,
                Operator.Xor,
                Operator.LeftShift,
                Operator.RightShift,
                Operator.UnsignedRightShift
            };


        public string[] GetBreakingBlockShortcutKeywords()
        {
            return breakingBlockShortcutKeywords;
        }

        public string[] GetNonNilLiterals()
        {
            return nonNilLiterals;
        }

        public string[] GetLiterals()
        {
            return literals;
        }

        public string[] GetExpressionOperators()
        {
            return expressionOperators;
        }

        public bool IsBreakingBlockShortcutKeyword(string? value)
        {
            return value != null && GetBreakingBlockShortcutKeywords().Contains(value);
        }

        public bool IsNonNilLiteral(string type)
        {
            return GetNonNilLiterals().Contains(type);
        }

        public bool IsLiteral(string type)
        {
            return GetLiterals().Contains(type);
        }

        public bool IsExpressionOperator(string value)
        {
            return GetExpressionOperators().Contains(value);
        }
    }
}
