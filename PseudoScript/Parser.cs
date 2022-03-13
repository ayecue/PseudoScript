using PseudoScript.Lexer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PseudoScript.Parser
{
    public class Options
    {
        public Lexer.Validator lexerValidator;
        public Validator validator;
        public AstProvider astProvider;
        public Lexer.Lexer lexer;
        public bool force;
        public int tabWidth;

        public Options(Validator? validator, AstProvider? astProvider, Lexer.Lexer? lexer, Lexer.Validator? lexerValidator, bool? force, int? tabWidth)
        {
            this.validator = validator ?? new Validator();
            this.astProvider = astProvider ?? new AstProvider();
            this.lexer = lexer;
            this.lexerValidator = lexerValidator;
            this.force = force ?? false;
            this.tabWidth = tabWidth ?? 1;
        }
    }

    public class Parser
    {
        static int GetPrecedence(string op)
        {
            return op switch
            {
                Operator.Or or Operator.BitwiseOr => 1,
                Operator.And or Operator.BitwiseAnd => 2,
                Operator.LessThan or Operator.GreaterThan or Operator.LessThanOrEqual or Operator.GreaterThanOrEqual or Operator.Equal or Operator.NotEqual => 3,
                Operator.LeftShift or Operator.RightShift or Operator.UnsignedRightShift => 7,
                Operator.Plus or Operator.Minus => 9,
                Operator.Asterik or Operator.PercentSign or Operator.Slash => 10,
                Operator.Xor => 12,
                _ => 0,
            };
        }

        readonly string content;
        readonly Lexer.Lexer lexer;
        readonly List<Lexer.Token> history;
        readonly Queue<Lexer.Token> prefetchedTokens;
        Lexer.Token? token;
        Lexer.Token? previousToken;
        readonly List<string> nativeImports;
        readonly HashSet<string> namespaces;
        readonly List<AstProvider.Base> literals;
        readonly Validator validator;
        readonly AstProvider astProvider;
        readonly bool force;
        readonly List<Exception> errors;

        //Pre allocated patterns
        static readonly string[] mapConstructorConsumeEOLPattern = new string[] { "}", "<eof>" };
        static readonly string[] parsePrimaryExpressionMapConsumePattern = new string[] { "{", "[" };
        static readonly string[] parseWhileStatementExpectedEOLPattern = new string[] { ";", "<eof>" };
        static readonly string[] parseIfShortcutStatementConsumeEOLPattern = new string[] { "end if", ";", "<eof>" };
        static readonly string[] parseForStatementExpectedEOLPattern = new string[] { ";", "<eof>" };
        static readonly string[] parseFunctionDeclarationExpectedEOLPattern = new string[] { ";", "<eof>" };

        public Parser(string content, Options options)
        {
            this.content = content;
            lexer = options.lexer ?? new Lexer.Lexer(content, new Lexer.Options(
                options.lexerValidator,
                options.force,
                options.tabWidth
            ));
            history = new List<Lexer.Token>();
            prefetchedTokens = new Queue<Lexer.Token>();
            token = null;
            previousToken = null;
            nativeImports = new List<string>();
            namespaces = new HashSet<string>();
            literals = new List<AstProvider.Base>();
            validator = options.validator;
            astProvider = options.astProvider;
            force = options.force;
            errors = new List<Exception>();
        }

        public Parser(string content) : this(content, new Options(null, null, null, null, null, null)) { }

        Parser Next()
        {
            if (previousToken != null)
            {
                history.Add(previousToken);
            }

            previousToken = token;
            token = Fetch();

            return this;
        }

        bool IsBlockFollow(Lexer.Token token)
        {
            string type = token.type;
            string value = token.value;
            if (Lexer.Token.Type.EOF == type) return true;
            if (Lexer.Token.Type.Keyword != type) return false;
            return value.StartsWith("else") || value.StartsWith("end");
        }

        bool Consume(string value)
        {
            if (token == null)
            {
                return false;
            }

            if (value == token.value && Lexer.Token.Type.StringLiteral != token.type)
            {
                Next();
                return true;
            }

            return false;
        }

        Lexer.Token Fetch()
        {
            Prefetch();

            if (prefetchedTokens.Count == 0)
            {
                return null;
            }

            return prefetchedTokens.Dequeue();
        }

        Lexer.Token Prefetch(int offset = 1)
        {

            int offsetIndex = offset - 1;

            while (prefetchedTokens.Count < offset)
            {
                Lexer.Token next = lexer.Next();

                if (next == null)
                {
                    return null;
                }

                prefetchedTokens.Enqueue(next);
                if (next.type == Lexer.Token.Type.EOF) break;
            }

            return prefetchedTokens.ElementAt(offsetIndex);
        }

        bool ConsumeMany(string[] values)
        {
            if (values.Contains(token.value) && Lexer.Token.Type.StringLiteral != token.type)
            {
                Next();
                return true;
            }

            return false;
        }

        void Expect(string value)
        {
            if (value == token.value && Lexer.Token.Type.StringLiteral != token.type)
            {
                Next();
            }
            else
            {
                Raise(new UnexpectedValue(token, value));
            }
        }

        void ExpectMany(string[] values)
        {


            if (values.Contains(token.value) && Lexer.Token.Type.StringLiteral != token.type)
            {
                Next();
            }
            else
            {
                Raise(new UnexpectedValues(token, values));
            }
        }

        bool IsUnary(Lexer.Token token)
        {
            string type = token.type;
            string value = token.value;

            return type switch
            {
                Lexer.Token.Type.Punctuator => "-" == value || "+" == value,
                Lexer.Token.Type.Keyword => "new" == value || "not" == value,
                _ => false,
            };
        }

        AstProvider.InvalidCodeExpression Raise(Exception err)
        {
            errors.Add(err);

            if (!force)
            {
                throw err;
            }

            AstProvider.Position start = new(token.line, token.lineRange.Start.Value);
            AstProvider.Position end = new(token.line, token.lineRange.End.Value);
            AstProvider.InvalidCodeExpression origin = astProvider.CreateInvalidCodeExpression(start, end);

            Next();

            while (
                token.type != Lexer.Token.Type.EOL &&
                token.type != Lexer.Token.Type.EOF
            )
            {
                Next();
            }

            return origin;
        }

        AstProvider.Base ParseIdentifier()
        {
            AstProvider.Position start = new(token.line, token.lineRange.Start.Value);
            AstProvider.Position end = new(token.line, token.lineRange.End.Value);
            string identifier = token.value;

            if (Lexer.Token.Type.Identifier != token.type)
            {
                return Raise(new UnexpectedIdentifier(token));
            }

            namespaces.Add(identifier);

            Next();

            return astProvider.CreateIdentifier(identifier, start, end);
        }

        AstProvider.MapConstructorExpression ParseMapConstructor()
        {
            AstProvider.Position start = new(previousToken.line, previousToken.lineRange.Start.Value);
            List<AstProvider.Base> fields = new();

            while (true)
            {
                if (token.type == Lexer.Token.Type.StringLiteral)
                {
                    string key = token.value;
                    AstProvider.Position startKey = new(token.line, token.lineRange.Start.Value);

                    Next();
                    Expect(":");

                    AstProvider.Base value = ParseExpectedExpression();

                    fields.Add(astProvider.CreateMapKeyString(
                        key,
                        value,
                        startKey,
                        value.end
                    ));
                }

                if (ConsumeMany(mapConstructorConsumeEOLPattern))
                {
                    break;
                }

                Next();
            }

            return astProvider.CreateMapConstructorExpression(fields, start, new AstProvider.Position(token.line, token.lineRange.Start.Value));
        }

        AstProvider.ListConstructorExpression ParseListConstructor()
        {
            AstProvider.Position start = new(previousToken.line, previousToken.lineRange.Start.Value);
            List<AstProvider.Base> fields = new();
            AstProvider.Base value;

            while (true)
            {
                value = ParseExpression();
                if (value != null) fields.Add(astProvider.CreateListValue(value, value.start, value.end));
                if (token.value == "," || token.value == ";")
                {
                    Next();
                    continue;
                }
                break;
            }

            Expect("]");

            return astProvider.CreateListConstructorExpression(fields, start, new AstProvider.Position(token.line, token.lineRange.Start.Value));
        }

        AstProvider.Base ParseRighthandExpressionGreedy(AstProvider.Base origin)
        {
            while (true)
            {
                AstProvider.Base newOrigin = ParseRighthandExpressionPart(origin);

                if (newOrigin == null) break;

                origin = newOrigin;
            }

            return origin;
        }

        AstProvider.Base ParseRighthandExpression()
        {
            AstProvider.Base origin;

            if (Lexer.Token.Type.Identifier == token.type)
            {
                origin = ParseIdentifier();
            }
            else if (Consume("("))
            {
                origin = ParseExpectedExpression();
                Expect(")");
            }
            else
            {
                return null;
            }

            return ParseRighthandExpressionGreedy(origin);
        }

        AstProvider.AssignmentStatement ParseAssignmentShorthandOperator(AstProvider.Base origin)
        {
            AstProvider.Position assignmentStart = origin.start;
            AstProvider.Position binaryExpressionStart = new(token.line, token.lineRange.Start.Value);
            string op = previousToken.value;
            AstProvider.Base value = ParseSubExpression();
            AstProvider.Position end = new(token.line, token.lineRange.End.Value);
            AstProvider.EvaluationExpression expression = astProvider.CreateEvaluationExpression(
                op,
                origin,
                value,
                binaryExpressionStart,
                end
            );

            return astProvider.CreateAssignmentStatement(
                origin,
                expression,
                assignmentStart,
                end
            );
        }

        AstProvider.IndexExpression ParseIndexExpression(AstProvider.Base origin)
        {
            AstProvider.Position start = new(token.line, token.lineRange.Start.Value);
            Lexer.Token currentToken = token;

            AstProvider.Base expression = ParseExpectedExpression();
            Expect("]");

            return astProvider.CreateIndexExpression(origin, expression, start, new AstProvider.Position(currentToken.line, currentToken.lineRange.End.Value));
        }

        AstProvider.Base ParseRighthandExpressionPart(AstProvider.Base origin)
        {
            AstProvider.Position start = new(token.line, token.lineRange.Start.Value);
            AstProvider.Base identifier;
            string type = token.type;

            if (Lexer.Token.Type.Punctuator == type)
            {
                string value = token.value;

                if (
                    Operator.AddShorthand == value ||
                    Operator.SubtractShorhand == value ||
                    Operator.MultiplyShorthand == value ||
                    Operator.DivideShorthand == value
                )
                {
                    Next();
                    return ParseAssignmentShorthandOperator(origin);
                }
                else if ("[" == value)
                {
                    Next();
                    return ParseIndexExpression(origin);
                }
                else if ("." == value)
                {
                    Next();
                    identifier = ParseIdentifier();
                    return astProvider.CreateMemberExpression(
                        origin,
                        ".",
                        identifier,
                        start,
                        new AstProvider.Position(token.line, token.lineRange.End.Value)
                    );
                }
                else if ("(" == value)
                {
                    return ParseCallExpression(origin);
                }
            }

            return null;
        }

        AstProvider.Base ParseCallExpression(AstProvider.Base origin)
        {
            AstProvider.Position start = new(token.line, token.lineRange.Start.Value);
            string value = token.value;

            if (Lexer.Token.Type.Punctuator != token.type || "(" != value)
            {
                return Raise(new UnexpectedArguments(token, origin));
            }

            if (token.line != previousToken.line)
            {
                return Raise(new CallExpressionEOL(token, previousToken));
            }

            Next();
            List<AstProvider.Base> expressions = new();
            AstProvider.Base expression = ParseExpression();

            if (null != expression) expressions.Add(expression);

            while (Consume(","))
            {
                expression = ParseExpectedExpression();
                expressions.Add(expression);
            }

            Expect(")");

            return astProvider.CreateCallExpression(
                origin,
                expressions,
                start,
                new AstProvider.Position(token.line, token.lineRange.End.Value)
            );
        }

        AstProvider.Base ParseFloatExpression()
        {
            return ParseFloatExpression(0);
        }

        AstProvider.Base ParseFloatExpression(double baseValue)
        {
            AstProvider.Position start = new(token.line, token.lineRange.Start.Value);

            Next();

            string floatValue = string.Format("{0}.{1}", (int)baseValue, token.value);

            Next();

            AstProvider.Base origin = astProvider.CreateLiteral(
                Token.Type.NumericLiteral,
                floatValue,
                floatValue,
                start,
                new AstProvider.Position(token.line, token.lineRange.End.Value)
            );

            literals.Add(origin);

            return origin;
        }

        AstProvider.Base ParsePrimaryExpression()
        {
            AstProvider.Position start = new(token.line, token.lineRange.Start.Value);
            object value = token.value;
            string type = token.type;

            if (validator.IsLiteral(type))
            {
                if (type == Lexer.Token.Type.BooleanLiteral)
                {
                    bool isBoolean = bool.TryParse(token.value, out bool boolResult);
                    value = isBoolean && boolResult;
                }
                else if (type == Lexer.Token.Type.NumericLiteral)
                {
                    bool isNumber = double.TryParse(token.value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double numberResult);
                    value = isNumber ? numberResult : 0;
                }
                else if (type == Lexer.Token.Type.NilLiteral)
                {
                    value = null;
                }

                string raw = content[token.range.Start.Value..token.range.End.Value];
                AstProvider.Base origin = astProvider.CreateLiteral(
                    type,
                    value,
                    raw,
                    start,
                    new AstProvider.Position(token.line, token.lineRange.End.Value)
                );

                literals.Add(origin);

                if (Lexer.Token.Type.NilLiteral != type && Prefetch()?.value == ".")
                {
                    Next();
                    origin = ParseRighthandExpressionGreedy(origin);
                }
                else
                {
                    Next();
                }

                return origin;
            }
            else if ("." == value && Token.Type.NumericLiteral == Prefetch().type)
            {
                return ParseFloatExpression(0);
            }
            else if (Lexer.Token.Type.Keyword == type && "function" == token.value)
            {
                Next();
                return ParseFunctionDeclaration();
            }
            else if (ConsumeMany(parsePrimaryExpressionMapConsumePattern))
            {
                AstProvider.Base origin;
                if ("{" == previousToken.value)
                {
                    origin = ParseMapConstructor();
                }
                else
                {
                    origin = ParseListConstructor();
                }

                origin = ParseRighthandExpressionGreedy(origin);

                return origin;
            }

            return null;
        }

        AstProvider.Base ParseBinaryExpression(AstProvider.Base expression, int minPrecedence = 0)
        {
            AstProvider.Position start = new(token.line, token.lineRange.Start.Value);
            int precedence;

            while (true)
            {
                string op = token.value;

                if (validator.IsExpressionOperator(op))
                {
                    precedence = GetPrecedence(op);
                }
                else
                {
                    precedence = 0;
                }

                if (precedence == 0 || precedence <= minPrecedence) break;
                if ("^" == op) --precedence;
                Next();

                AstProvider.Base right = ParseSubExpression(precedence);

                if (null == right)
                {
                    right = astProvider.CreateEmptyExpression(start, new AstProvider.Position(token.line, token.lineRange.End.Value));
                }

                expression = astProvider.CreateEvaluationExpression(
                    op,
                    expression,
                    right,
                    start,
                    new AstProvider.Position(token.line, token.lineRange.End.Value)
                );
            }

            return expression;
        }

        AstProvider.Base ParseSubExpression(int minPrecedence = 0)
        {
            AstProvider.Position start = new(token.line, token.lineRange.Start.Value);
            string op = token.value;
            AstProvider.Base expression = null;

            if (IsUnary(token))
            {
                Next();

                AstProvider.Base argument = ParsePrimaryExpression();

                if (argument == null)
                {
                    argument = ParseRighthandExpression();
                }

                expression = astProvider.CreateUnaryExpression(
                    op,
                    argument,
                    start,
                    new AstProvider.Position(token.line, token.lineRange.End.Value)
                );
            }
            if (expression == null)
            {
                expression = ParsePrimaryExpression();

                if (expression == null)
                {
                    expression = ParseRighthandExpression();
                }
            }

            expression = ParseBinaryExpression(expression, minPrecedence);

            return expression;
        }

        AstProvider.Base ParseImportStatement()
        {
            AstProvider.Position start = new(previousToken.line, previousToken.lineRange.Start.Value);

            string directory;

            if (Lexer.Token.Type.StringLiteral == token.type)
            {
                directory = token.value;
                Next();
            }
            else
            {
                return Raise(new UnexpectedImport(token));
            }

            Expect(";");

            return astProvider.CreateImportExpression(
                directory,
                start,
                new AstProvider.Position(token.line, token.lineRange.End.Value)
            );
        }

        AstProvider.WhileStatement ParseWhileStatement()
        {
            AstProvider.Position start = new(previousToken.line, previousToken.lineRange.Start.Value);
            AstProvider.Base condition = ParseExpectedExpression();

            List<AstProvider.Base> body;

            if (Lexer.Token.Type.EOL == token.type)
            {
                body = ParseBlock();
                Expect("end while");
            }
            else
            {
                body = ParseBlockShortcut();
                ExpectMany(parseWhileStatementExpectedEOLPattern);
            }

            return astProvider.CreateWhileStatement(
                condition,
                body,
                start,
                new AstProvider.Position(previousToken.line, previousToken.lineRange.End.Value)
            );
        }

        AstProvider.Base ParseExpression()
        {
            return ParseSubExpression();
        }

        AstProvider.Base ParseExpectedExpression()
        {
            AstProvider.Base expression = ParseExpression();

            if (expression == null)
            {
                return Raise(new UnexpectedExpression(token));
            }

            return expression;
        }

        AstProvider.Base ParseIfShortcutStatement(AstProvider.Base condition, AstProvider.Position start)
        {
            List<AstProvider.Base> clauses = new();
            AstProvider.Position statementStart;
            List<AstProvider.Base> body = ParseBlockShortcut();

            clauses.Add(astProvider.CreateIfClause(
                condition,
                body,
                start,
                new AstProvider.Position(token.line, token.lineRange.End.Value)
            ));

            Consume(";");

            while (Consume("else if"))
            {
                statementStart = new AstProvider.Position(token.line, token.lineRange.Start.Value);
                condition = ParseExpectedExpression();
                Expect("then");
                body = ParseBlockShortcut();

                clauses.Add(astProvider.CreateElseifClause(
                    condition,
                    body,
                    statementStart,
                    new AstProvider.Position(token.line, token.lineRange.End.Value)
                ));

                Consume(";");
            }

            if (Consume("else"))
            {
                statementStart = new AstProvider.Position(token.line, token.lineRange.Start.Value);
                body = ParseBlockShortcut();

                clauses.Add(astProvider.CreateElseClause(
                    body,
                    statementStart,
                    new AstProvider.Position(token.line, token.lineRange.End.Value)
                ));

                Consume(";");
            }

            ConsumeMany(parseIfShortcutStatementConsumeEOLPattern);

            return astProvider.CreateIfStatement(
                clauses,
                start,
                new AstProvider.Position(previousToken.line, previousToken.lineRange.End.Value)
            ); ;
        }

        AstProvider.Base ParseIfStatement()
        {
            List<AstProvider.Base> clauses = new();
            AstProvider.Position start = new(previousToken.line, previousToken.lineRange.Start.Value);
            AstProvider.Position statementStart = start;
            AstProvider.Base condition;
            List<AstProvider.Base> body;

            condition = ParseExpectedExpression();
            Expect("then");

            if (Lexer.Token.Type.EOL != token.type) return ParseIfShortcutStatement(condition, start);

            body = ParseBlock();
            clauses.Add(astProvider.CreateIfClause(condition, body, statementStart, new AstProvider.Position(token.line, token.lineRange.End.Value)));

            while (Consume("else if"))
            {
                statementStart = new AstProvider.Position(token.line, token.lineRange.Start.Value);
                condition = ParseExpectedExpression();
                Expect("then");
                body = ParseBlock();
                clauses.Add(astProvider.CreateElseifClause(condition, body, statementStart, new AstProvider.Position(token.line, token.lineRange.End.Value)));
            }

            if (Consume("else"))
            {
                statementStart = new AstProvider.Position(token.line, token.lineRange.Start.Value);
                body = ParseBlock();
                clauses.Add(astProvider.CreateElseClause(body, statementStart, new AstProvider.Position(token.line, token.lineRange.End.Value)));
            }

            Expect("end if");

            return astProvider.CreateIfStatement(
                clauses,
                start,
                new AstProvider.Position(previousToken.line, previousToken.lineRange.End.Value)
            );
        }

        AstProvider.ReturnStatement ParseReturnStatement(bool isShortcutStatement = false)
        {
            AstProvider.Position start = new(previousToken.line, previousToken.lineRange.Start.Value);
            AstProvider.Base expression = ParseExpression();

            if (!isShortcutStatement) Consume(";");

            return astProvider.CreateReturnStatement(
                expression,
                start,
                new AstProvider.Position(token.line, token.lineRange.End.Value)
            );
        }

        AstProvider.Base ParseAssignmentOrCallStatement()
        {
            AstProvider.Position start = new(token.line, token.lineRange.Start.Value);
            AstProvider.Base origin;
            Lexer.Token last = token;

            if (Lexer.Token.Type.Identifier == last.type)
            {
                origin = ParseIdentifier();
            }
            else if ("(" == last.value)
            {
                Next();
                origin = ParseExpectedExpression();
                Expect(")");
            }
            else if (validator.IsNonNilLiteral(last.type))
            {
                origin = ParseExpectedExpression();
            }
            else if ("[" == token.value || "{" == last.value)
            {
                origin = ParseExpectedExpression();
            }
            else
            {
                return Raise(new UnexpectedAssignmentOrCall(token));
            }

            if (validator.IsExpressionOperator(token.value))
            {
                return ParseBinaryExpression(origin);
            }

            while (
                Lexer.Token.Type.Punctuator == token.type &&
                "=" != token.value &&
                ";" != token.value &&
                ")" != token.value &&
                "<eof>" != token.value
            )
            {
                last = token;
                origin = ParseRighthandExpressionGreedy(origin);
            }

            if (
                token.type == Lexer.Token.Type.EOL ||
                token.type == Lexer.Token.Type.EOF ||
                token.value == "else"
            )
            {
                if (validator.IsLiteral(last.type))
                {
                    return origin;
                }

                return astProvider.CreateCallStatement(origin, start, new AstProvider.Position(token.line, token.lineRange.End.Value));
            }

            Expect("=");

            AstProvider.Base value = ParseExpectedExpression();

            return astProvider.CreateAssignmentStatement(
                origin,
                value,
                start,
                new AstProvider.Position(token.line, token.lineRange.End.Value)
            );
        }

        AstProvider.ForGenericStatement ParseForStatement()
        {
            AstProvider.Position start = new(previousToken.line, previousToken.lineRange.Start.Value);

            Consume("(");

            AstProvider.Base variable = ParseIdentifier();

            Expect("in");

            AstProvider.Base iterator = ParseExpectedExpression();

            Consume(")");

            List<AstProvider.Base> body;

            if (Lexer.Token.Type.EOL == token.type)
            {
                body = ParseBlock();
                Expect("end for");
            }
            else
            {
                body = ParseBlockShortcut();
                ExpectMany(parseForStatementExpectedEOLPattern);
            }

            return astProvider.CreateForGenericStatement(
                variable,
                iterator,
                body,
                start,
                new AstProvider.Position(previousToken.line, previousToken.lineRange.End.Value)
            );
        }

        AstProvider.Base ParseFunctionDeclaration()
        {

            AstProvider.Position start = new(previousToken.line, previousToken.lineRange.Start.Value);
            List<AstProvider.Base> parameters = new();
            string name = "anonymous";

            if (token.type == Lexer.Token.Type.Identifier)
            {
                AstProvider.Identifier fnIdentifier = (AstProvider.Identifier)ParseIdentifier();
                name = fnIdentifier.name;
            }

            Expect("(");

            if (!Consume(")"))
            {
                while (true)
                {
                    if (Lexer.Token.Type.Identifier == token.type)
                    {
                        AstProvider.Base parameter = ParseIdentifier();
                        AstProvider.Position parameterStart = parameter.start;

                        if (Consume("="))
                        {
                            AstProvider.Base value = ParseExpectedExpression();
                            parameter = astProvider.CreateAssignmentStatement(
                                parameter,
                                value,
                                parameterStart,
                                new AstProvider.Position(token.line, token.lineRange.End.Value)
                            );
                        }

                        parameters.Add(parameter);
                        if (Consume(",")) continue;
                    }
                    else
                    {
                        return Raise(new UnexpectedParameterInFunction(token));
                    }

                    Expect(")");
                    break;
                }
            }

            List<AstProvider.Base> body;

            if (Lexer.Token.Type.EOL == token.type)
            {
                body = ParseBlock();
                Expect("end function");
            }
            else
            {
                body = ParseBlockShortcut();
                ExpectMany(parseFunctionDeclarationExpectedEOLPattern);
            }

            if (name != null)
            {

            }

            return astProvider.CreateFunctionDeclaration(
                name,
                parameters,
                body,
                start,
                new AstProvider.Position(previousToken.line, previousToken.lineRange.End.Value)
            );
        }

        AstProvider.Base ParseStatement(bool isShortcutStatement = false)
        {


            if (Lexer.Token.Type.Keyword == token.type)
            {
                string value = token.value;

                switch (value)
                {
                    case "if":
                        Next();
                        return ParseIfStatement();
                    case "return":
                        Next();
                        return ParseReturnStatement(isShortcutStatement);
                    case "function":
                        Next();
                        return ParseFunctionDeclaration();
                    case "while":
                        Next();
                        return ParseWhileStatement();
                    case "for":
                        Next();
                        return ParseForStatement();
                    case "continue":
                        Next();
                        return astProvider.CreateContinueStatement(new AstProvider.Position(previousToken.line, previousToken.lineRange.Start.Value), new AstProvider.Position(previousToken.line, previousToken.lineRange.End.Value));
                    case "break":
                        Next();
                        return astProvider.CreateBreakStatement(new AstProvider.Position(previousToken.line, previousToken.lineRange.Start.Value), new AstProvider.Position(previousToken.line, previousToken.lineRange.End.Value));
                    case "import":
                        Next();
                        return ParseImportStatement();
                    case "debugger":
                        Next();
                        return astProvider.CreateDebuggerStatement(new AstProvider.Position(previousToken.line, previousToken.lineRange.Start.Value), new AstProvider.Position(previousToken.line, previousToken.lineRange.End.Value));
                    default:
                        break;
                }
            }
            else if (Lexer.Token.Type.EOL == token.type)
            {
                Next();
                return null;
            }

            return ParseAssignmentOrCallStatement();
        }

        List<AstProvider.Base> ParseBlockShortcut()
        {

            List<AstProvider.Base> block = new();
            string value = token.value;

            while (
                token.type != Lexer.Token.Type.EOL &&
                !validator.IsBreakingBlockShortcutKeyword(value)
            )
            {
                AstProvider.Base statement = ParseStatement("return" == value);
                if (statement != null) block.Add(statement);
                value = token.value;
            }

            return block;
        }

        List<AstProvider.Base> ParseBlock()
        {
            List<AstProvider.Base> block = new();

            while (!IsBlockFollow(token))
            {
                AstProvider.Base statement = ParseStatement();
                Consume(";");
                if (statement != null) block.Add(statement);
            }

            return block;
        }

        public AstProvider.Base ParseChunk()
        {
            Next();

            AstProvider.Position start = new(token.line, token.lineRange.Start.Value);
            List<AstProvider.Base> body = ParseBlock();

            if (Lexer.Token.Type.EOF != token.type)
            {
                return Raise(new UnexpectedEOF(token));
            }

            return astProvider.CreateChunk(
                body,
                nativeImports,
                namespaces,
                literals,
                start,
                new AstProvider.Position(token.line, token.lineRange.End.Value)
            );
        }
    }
}
