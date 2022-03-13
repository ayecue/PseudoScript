using System;
using System.Collections.Generic;

namespace PseudoScript.Lexer
{
    public class Options
    {
        public Validator validator;
        public bool force;
        public int tabWidth;

        public Options(Validator? validator, bool? force, int? tabWidth)
        {
            this.validator = validator ?? new Validator();
            this.force = force ?? false;
            this.tabWidth = tabWidth ?? 1;
        }
    }

    public class Lexer
    {
        readonly string content;
        readonly int length;
        int index;
        int tokenStart;
        int line;
        int lineStart;
        int offset;
        readonly int tabWidth;

        readonly Validator validator;
        readonly bool force;
        readonly List<Exception> errors;

        public Lexer(string content, Options? options)
        {
            this.content = content;
            length = content.Length;
            index = 0;
            tokenStart = 0;
            tabWidth = options.tabWidth;
            line = 1;
            lineStart = 0;
            offset = 0;
            validator = options.validator;
            force = options.force;
            errors = new List<Exception>();
        }

        public Lexer(string content) : this(content, new Options(null, null, null)) { }

        public Token Scan(CharacterCode code, CharacterCode? nextCode, CharacterCode? lastCode)
        {
            switch (code)
            {
                case CharacterCode.QUOTE:
                    return ScanStringLiteral();
                case CharacterCode.DOT:
                    if (validator.IsDecDigit(nextCode ?? CharacterCode.UNKNOWN)) return ScanNumericLiteral();
                    return ScanPunctuator(".");
                case CharacterCode.EQUAL:
                    if (CharacterCode.EQUAL == nextCode) return ScanPunctuator("==");
                    return ScanPunctuator("=");
                case CharacterCode.ARROW_LEFT:
                    if (CharacterCode.EQUAL == nextCode) return ScanPunctuator("<=");
                    if (CharacterCode.ARROW_LEFT == nextCode) return ScanPunctuator("<<");
                    return ScanPunctuator("<");
                case CharacterCode.ARROW_RIGHT:
                    if (CharacterCode.EQUAL == nextCode) return ScanPunctuator(">=");
                    if (CharacterCode.ARROW_RIGHT == nextCode)
                    {
                        if (CharacterCode.ARROW_RIGHT == lastCode) return ScanPunctuator(">>>");
                        return ScanPunctuator(">>");
                    }
                    return ScanPunctuator(">");
                case CharacterCode.EXCLAMATION_MARK:
                    if (CharacterCode.EQUAL == nextCode) return ScanPunctuator("!=");
                    return null;
                case CharacterCode.MINUS:
                    if (CharacterCode.EQUAL == nextCode) return ScanPunctuator("-=");
                    return ScanPunctuator("-");
                case CharacterCode.PLUS:
                    if (CharacterCode.EQUAL == nextCode) return ScanPunctuator("+=");
                    return ScanPunctuator("+");
                case CharacterCode.ASTERISK:
                    if (CharacterCode.EQUAL == nextCode) return ScanPunctuator("*=");
                    return ScanPunctuator("*");
                case CharacterCode.SLASH:
                    if (CharacterCode.EQUAL == nextCode) return ScanPunctuator("/=");
                    return ScanPunctuator("/");
                case CharacterCode.CARET:
                case CharacterCode.PERCENT:
                case CharacterCode.COLON:
                case CharacterCode.COMMA:
                case CharacterCode.CURLY_BRACKET_LEFT:
                case CharacterCode.CURLY_BRACKET_RIGHT:
                case CharacterCode.SQUARE_BRACKETS_LEFT:
                case CharacterCode.SQUARE_BRACKETS_RIGHT:
                case CharacterCode.PARENTHESIS_LEFT:
                case CharacterCode.PARENTHESIS_RIGHT:
                case CharacterCode.AMPERSAND:
                case CharacterCode.VERTICAL_LINE:
                    return ScanPunctuator(((char)code).ToString());
                case CharacterCode.NUMBER_0:
                case CharacterCode.NUMBER_1:
                case CharacterCode.NUMBER_2:
                case CharacterCode.NUMBER_3:
                case CharacterCode.NUMBER_4:
                case CharacterCode.NUMBER_5:
                case CharacterCode.NUMBER_6:
                case CharacterCode.NUMBER_7:
                case CharacterCode.NUMBER_8:
                case CharacterCode.NUMBER_9:
                    return ScanNumericLiteral();
                case CharacterCode.SEMICOLON:
                    NextIndex();
                    return CreateEOL();
                default:
                    return null;
            }
        }

        bool IsNotEOF()
        {
            return index < length;
        }

        int NextIndex(int value = 1)
        {
            index += value;
            return index;
        }

        CharacterCode CodeAt(int codeOffset = 0)
        {
            int position = index + codeOffset;

            if (position >= content.Length)
            {
                return CharacterCode.SEMICOLON;
            }

            return (CharacterCode)char.ConvertToUtf32(content, position);
        }


        int NextLine()
        {
            return ++line;
        }

        bool IsStringEscaped()
        {
            return CharacterCode.QUOTE == CodeAt(1);
        }

        Token CreateEOL()
        {
            return new Token(
                Token.Type.EOL,
                ";",
                line,
                lineStart,
                new Range(
                    tokenStart,
                    index
                ),
                offset,
                null,
                null
            );
        }

        Token ScanStringLiteral()
        {
            int beginLine = line;
            int beginLineStart = lineStart;
            int stringStart = index + 1;
            CharacterCode code;

            while (true)
            {
                NextIndex();
                code = CodeAt();
                if (CharacterCode.QUOTE == code)
                {
                    if (IsStringEscaped())
                    {
                        NextIndex();
                    }
                    else
                    {
                        break;
                    }
                }
                if (!IsNotEOF())
                {
                    Raise(new UnexpectedStringEOL(beginLine));
                    return null;
                }
            }

            NextIndex();
            string str = content.Substring(stringStart, index - stringStart - 1).Replace("\"\"", "\"");

            return new Token(
                Token.Type.StringLiteral,
                str,
                beginLine,
                beginLineStart,
                new Range(
                    tokenStart,
                    index
                ),
                offset,
                line,
                lineStart
            );
        }

        Token ScanNumericLiteral()
        {
            while (validator.IsDecDigit(CodeAt())) NextIndex();

            if (CharacterCode.DOT == CodeAt())
            {
                NextIndex();
                while (validator.IsDecDigit(CodeAt())) NextIndex();
            }

            CharacterCode notation = CodeAt();
            if (CharacterCode.LETTER_E == notation || CharacterCode.LETTER_e == notation)
            {
                NextIndex();
                CharacterCode operation = CodeAt();
                if (CharacterCode.MINUS == operation || CharacterCode.PLUS == operation) NextIndex();
                while (validator.IsDecDigit(CodeAt())) NextIndex();
            }

            return new Token(
                Token.Type.NumericLiteral,
                content[tokenStart..index],
                line,
                lineStart,
                new Range(
                    tokenStart,
                    index
                ),
                offset
            );
        }

        Token ScanPunctuator(string value)
        {
            index += value.Length;

            return new Token(
                Token.Type.Punctuator,
                value,
                line,
                lineStart,
                new Range(
                    tokenStart,
                    index
                ),
                offset
            );
        }

        void SkipToNextLine()
        {
            CharacterCode code = CodeAt();

            while (!validator.IsEndOfLine(code) && !IsNotEOF())
            {
                NextIndex();
                code = CodeAt();
            }

            NextLine();
            offset = index;

            Next();
        }

        void SkipWhiteSpace()
        {
            while (IsNotEOF())
            {
                CharacterCode code = CodeAt();
                if (code == CharacterCode.WHITESPACE)
                {
                    NextIndex();
                }
                else if (code == CharacterCode.TAB)
                {
                    offset -= tabWidth - 1;
                    NextIndex();
                }
                else
                {
                    break;
                }
            }
        }

        Token ScanIdentifierOrKeyword()
        {
            NextIndex();

            while (validator.IsIdentifierPart(CodeAt()))
            {
                NextIndex();
            }

            string value = content[tokenStart..index];
            string type;

            if (validator.IsKeyword(value))
            {
                type = Token.Type.Keyword;

                if ("end" == value)
                {
                    NextIndex();

                    while (validator.IsIdentifierPart(CodeAt()))
                    {
                        NextIndex();
                    }
                    value = content[tokenStart..index];
                }
                else if ("else" == value)
                {
                    string elseIfStatement = content.Substring(tokenStart, index - tokenStart + 3);

                    if ("else if" == elseIfStatement)
                    {
                        NextIndex(3);
                        value = elseIfStatement;
                    }
                }
            }
            else if ("true" == value || "false" == value)
            {
                return new Token(
                    Token.Type.BooleanLiteral,
                    value,
                    line,
                    lineStart,
                    new Range(
                        tokenStart,
                        index
                    ),
                    offset
                );
            }
            else if ("null" == value)
            {
                return new Token(
                    Token.Type.NilLiteral,
                    value,
                    line,
                    lineStart,
                    new Range(
                        tokenStart,
                        index
                    ),
                    offset
                );
            }
            else
            {
                type = Token.Type.Identifier;
            }

            return new Token(
                type,
                value,
                line,
                lineStart,
                new Range(
                    tokenStart,
                    index
                ),
                offset
            );
        }

        void ScanComment()
        {
            while (IsNotEOF())
            {
                if (validator.IsEndOfLine(CodeAt())) break;
                NextIndex();
            }
        }

        public Token Next()
        {
            SkipWhiteSpace();

            while (validator.IsComment(CodeAt(), CodeAt(1)))
            {
                tokenStart = index;
                ScanComment();
            }

            if (!IsNotEOF())
            {
                return new Token(
                    Token.Type.EOF,
                    "<eof>",
                    line,
                    lineStart,
                    new Range(
                        index,
                        index
                    ),
                    offset
                );
            }

            CharacterCode code = CodeAt();
            CharacterCode nextCode = CodeAt(1);
            CharacterCode lastCode = CodeAt(2);

            tokenStart = index;

            if (validator.IsEndOfLine(code))
            {
                if (CharacterCode.NEW_LINE == code && CharacterCode.RETURN_LINE == nextCode) NextIndex();
                if (CharacterCode.RETURN_LINE == code && CharacterCode.NEW_LINE == nextCode) NextIndex();

                Token token = CreateEOL();

                NextLine();
                offset = index + 1;
                lineStart = NextIndex();

                return token;
            }

            if (validator.IsIdentifierStart(code)) return ScanIdentifierOrKeyword();

            Token item = Scan(code, nextCode, lastCode);

            if (item != null) return item;

            Raise(new InvalidCharacter(code, line));

            return null;
        }

        void Raise(Exception err)
        {
            errors.Add(err);

            if (force)
            {
                SkipToNextLine();
                Next();
                return;
            }

            throw err;
        }
    }
}
