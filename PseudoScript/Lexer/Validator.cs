using System;
using System.Linq;

namespace PseudoScript.Lexer
{
    public class Validator
    {
        static readonly string[] keywordsTwo = new string[] { "if", "in", "or" };
        static readonly string[] keywordsThree = new string[] { "and", "end", "for", "not", "new" };
        static readonly string[] keywordsFour = new string[] { "else", "then" };
        static readonly string[] keywordsFive = new string[] { "break", "while" };
        static readonly string[] keywordsSix = new string[] { "return", "import" };
        static readonly string[] keywordsEight = new string[] { "function", "continue", "debugger" };
        static readonly string[] keywordsNil = Array.Empty<string>();

        public string[] GetKeywords(int index)
        {
            return index switch
            {
                2 => keywordsTwo,
                3 => keywordsThree,
                4 => keywordsFour,
                5 => keywordsFive,
                6 => keywordsSix,
                8 => keywordsEight,
                _ => keywordsNil,
            };
        }

        public bool IsKeyword(string value)
        {
            int length = value.Length;
            string[] keywords = this.GetKeywords(length);

            return keywords.Contains(value);
        }

        public bool IsWhiteSpace(CharacterCode code)
        {
            return CharacterCode.WHITESPACE == code || CharacterCode.TAB == code;
        }

        public bool IsEndOfLine(CharacterCode code)
        {
            return CharacterCode.NEW_LINE == code || CharacterCode.RETURN_LINE == code;
        }

        public bool IsComment(CharacterCode code, CharacterCode nextCode)
        {
            return CharacterCode.SLASH == code && CharacterCode.SLASH == nextCode;
        }

        public bool IsIdentifierStart(CharacterCode code)
        {
            int value = (int)code;
            return (value >= 65 && value <= 90) ||
                (value >= 97 && value <= 122) ||
                95 == value || value >= 128;
        }

        public bool IsIdentifierPart(CharacterCode code)
        {
            int value = (int)code;
            return (value >= 65 && value <= 90) ||
                (value >= 97 && value <= 122) || 95 == value ||
                (value >= 48 && value <= 57) || value >= 128;
        }

        public bool IsDecDigit(CharacterCode code)
        {
            return code >= CharacterCode.NUMBER_0 && code <= CharacterCode.NUMBER_9;
        }
    }
}
