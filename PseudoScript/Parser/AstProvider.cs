using System.Collections.Generic;

namespace PseudoScript.Parser
{
    public class AstProvider
    {
        static public class Type
        {
            public const string BreakStatement = "BreakStatement";
            public const string ContinueStatement = "ContinueStatement";
            public const string ReturnStatement = "ReturnStatement";
            public const string IfStatement = "IfStatement";
            public const string IfClause = "IfClause";
            public const string ElseifClause = "ElseifClause";
            public const string ElseClause = "ElseClause";
            public const string WhileStatement = "WhileStatement";
            public const string AssignmentStatement = "AssignmentStatement";
            public const string CallStatement = "CallStatement";
            public const string FunctionDeclaration = "FunctionDeclaration";
            public const string ForGenericStatement = "ForGenericStatement";
            public const string Chunk = "Chunk";
            public const string Identifier = "Identifier";
            public const string StringLiteral = "StringLiteral";
            public const string NumericLiteral = "NumericLiteral";
            public const string BooleanLiteral = "BooleanLiteral";
            public const string NilLiteral = "NilLiteral";
            public const string MemberExpression = "MemberExpression";
            public const string CallExpression = "CallExpression";
            public const string Comment = "Comment";
            public const string NegationExpression = "NegationExpression";
            public const string BinaryNegatedExpression = "BinaryNegatedExpression";
            public const string UnaryExpression = "UnaryExpression";
            public const string MapKeyString = "MapKeyString";
            public const string MapValue = "MapValue";
            public const string MapConstructorExpression = "MapConstructorExpression";
            public const string MapCallExpression = "MapCallExpression";
            public const string ListValue = "ListValue";
            public const string ListConstructorExpression = "ListConstructorExpression";
            public const string EmptyExpression = "EmptyExpression";
            public const string IndexExpression = "IndexExpression";
            public const string BinaryExpression = "BinaryExpression";
            public const string LogicalExpression = "LogicalExpression";
            public const string ImportExpression = "ImportExpression";
            public const string InvalidCodeExpression = "InvalidCodeExpression";
            public const string DebuggerStatement = "DebuggerStatement";
        }

        public class Position
        {
            public readonly int line;
            public readonly int character;

            public Position(int line, int character)
            {
                this.line = line;
                this.character = character;
            }
        }

        public class Base
        {
            public readonly string type;
            public readonly Position start;
            public readonly Position end;

            public Base(string type, Position start, Position end)
            {
                this.type = type;
                this.start = start;
                this.end = end;
            }
        }

        public class DebuggerStatement : Base
        {
            public DebuggerStatement(Position start, Position end) : base(Type.DebuggerStatement, start, end) { }
        }

        public class BreakStatement : Base
        {
            public BreakStatement(Position start, Position end) : base(Type.BreakStatement, start, end) { }
        }

        public class ContinueStatement : Base
        {
            public ContinueStatement(Position start, Position end) : base(Type.ContinueStatement, start, end) { }
        }

        public class ReturnStatement : Base
        {
            public readonly Base arg;

            public ReturnStatement(Base arg, Position start, Position end) : base(Type.ReturnStatement, start, end)
            {
                this.arg = arg;
            }
        }

        public class IfStatement : Base
        {
            public List<Base> clauses;

            public IfStatement(List<Base> clauses, Position start, Position end) : base(Type.IfStatement, start, end)
            {
                this.clauses = clauses;
            }
        }

        public class IfClause : Base
        {
            public readonly Base condition;
            public readonly List<Base> body;

            public IfClause(Base condition, List<Base> body, Position start, Position end) : base(Type.IfClause, start, end)
            {
                this.condition = condition;
                this.body = body;
            }
        }

        public class ElseifClause : Base
        {
            public readonly Base condition;
            public readonly List<Base> body;

            public ElseifClause(Base condition, List<Base> body, Position start, Position end) : base(Type.ElseifClause, start, end)
            {
                this.condition = condition;
                this.body = body;
            }
        }

        public class ElseClause : Base
        {
            public readonly List<Base> body;

            public ElseClause(List<Base> body, Position start, Position end) : base(Type.ElseClause, start, end)
            {
                this.body = body;
            }
        }

        public class WhileStatement : Base
        {
            public readonly Base condition;
            public readonly List<Base> body;

            public WhileStatement(Base condition, List<Base> body, Position start, Position end) : base(Type.WhileStatement, start, end)
            {
                this.condition = condition;
                this.body = body;
            }
        }

        public class AssignmentStatement : Base
        {
            public readonly Base variable;
            public readonly Base init;

            public AssignmentStatement(Base variable, Base init, Position start, Position end) : base(Type.AssignmentStatement, start, end)
            {
                this.variable = variable;
                this.init = init;
            }
        }

        public class CallStatement : Base
        {
            public readonly Base expression;

            public CallStatement(Base expression, Position start, Position end) : base(Type.CallStatement, start, end)
            {
                this.expression = expression;
            }
        }

        public class FunctionDeclaration : Base
        {
            public readonly List<Base> arguments;
            public readonly List<Base> body;
            public readonly string name;

            public FunctionDeclaration(string name, List<Base> arguments, List<Base> body, Position start, Position end) : base(Type.FunctionDeclaration, start, end)
            {
                this.name = name;
                this.arguments = arguments;
                this.body = body;
            }
        }

        public class ForGenericStatement : Base
        {
            public readonly Base variable;
            public readonly Base iterator;
            public readonly List<Base> body;

            public ForGenericStatement(Base variable, Base iterator, List<Base> body, Position start, Position end) : base(Type.ForGenericStatement, start, end)
            {
                this.variable = variable;
                this.iterator = iterator;
                this.body = body;
            }
        }

        public class Chunk : Base
        {
            public readonly List<Base> body;
            public readonly List<string> imports;
            public readonly HashSet<string> namespaces;
            public readonly List<Base> literals;

            public Chunk(List<Base> body, List<string> imports, HashSet<string> namespaces, List<Base> literals, Position start, Position end) : base(Type.Chunk, start, end)
            {
                this.body = body;
                this.imports = imports;
                this.namespaces = namespaces;
                this.literals = literals;
            }
        }

        public class Identifier : Base
        {
            public readonly string name;

            public Identifier(string name, Position start, Position end) : base(Type.Identifier, start, end)
            {
                this.name = name;
            }
        }

        public class Literal : Base
        {
            public readonly object value;
            public readonly string raw;

            public Literal(string type, object value, string raw, Position start, Position end) : base(type, start, end)
            {
                this.value = value;
                this.raw = raw;
            }
        }

        public class MemberExpression : Base
        {
            public readonly Base origin;
            public readonly string indexer;
            public readonly Base identifier;

            public MemberExpression(Base origin, string indexer, Base identifier, Position start, Position end) : base(Type.MemberExpression, start, end)
            {
                this.origin = origin;
                this.indexer = indexer;
                this.identifier = identifier;
            }
        }

        public class CallExpression : Base
        {
            public readonly Base origin;
            public readonly List<Base> arguments;

            public CallExpression(Base origin, List<Base> arguments, Position start, Position end) : base(Type.CallExpression, start, end)
            {
                this.origin = origin;
                this.arguments = arguments;
            }
        }

        public class Comment : Base
        {
            public readonly string value;
            public readonly string raw;

            public Comment(string value, string raw, Position start, Position end) : base(Type.Comment, start, end)
            {
                this.value = value;
                this.raw = raw;
            }
        }

        public class UnaryExpression : Base
        {
            public readonly string? op;
            public readonly Base arg;

            public UnaryExpression(string type, string op, Base arg, Position start, Position end) : base(type, start, end)
            {
                this.op = op;
                this.arg = arg;
            }
        }

        public class MapKeyString : Base
        {
            public readonly Base value;
            public readonly string key;

            public MapKeyString(string key, Base value, Position start, Position end) : base(Type.MapKeyString, start, end)
            {
                this.value = value;
                this.key = key;
            }
        }

        public class ListValue : Base
        {
            public readonly Base value;

            public ListValue(Base value, Position start, Position end) : base(Type.ListValue, start, end)
            {
                this.value = value;
            }
        }

        public class MapConstructorExpression : Base
        {
            public readonly List<Base> fields;

            public MapConstructorExpression(List<Base> fields, Position start, Position end) : base(Type.MapConstructorExpression, start, end)
            {
                this.fields = fields;
            }
        }

        public class ListConstructorExpression : Base
        {
            public readonly List<Base> fields;

            public ListConstructorExpression(List<Base> fields, Position start, Position end) : base(Type.ListConstructorExpression, start, end)
            {
                this.fields = fields;
            }
        }

        public class EmptyExpression : Base
        {
            public EmptyExpression(Position start, Position end) : base(Type.EmptyExpression, start, end) { }
        }

        public class InvalidCodeExpression : Base
        {
            public InvalidCodeExpression(Position start, Position end) : base(Type.InvalidCodeExpression, start, end) { }
        }

        public class IndexExpression : Base
        {
            public readonly Base origin;
            public readonly Base index;

            public IndexExpression(Base origin, Base index, Position start, Position end) : base(Type.IndexExpression, start, end)
            {
                this.origin = origin;
                this.index = index;
            }
        }

        public class EvaluationExpression : Base
        {
            public readonly string op;
            public readonly Base left;
            public readonly Base right;

            public EvaluationExpression(string type, string op, Base left, Base right, Position start, Position end) : base(type, start, end)
            {
                this.op = op;
                this.left = left;
                this.right = right;
            }
        }

        public class ImportExpression : Base
        {
            public readonly string directory;

            public ImportExpression(string directory, Position start, Position end) : base(Type.ImportExpression, start, end)
            {
                this.directory = directory;
            }
        }

        public BreakStatement CreateBreakStatement(Position start, Position end)
        {
            return new BreakStatement(start, end);
        }

        public ContinueStatement CreateContinueStatement(Position start, Position end)
        {
            return new ContinueStatement(start, end);
        }

        public ReturnStatement CreateReturnStatement(Base arg, Position start, Position end)
        {
            return new ReturnStatement(arg, start, end);
        }

        public IfStatement CreateIfStatement(List<Base> clauses, Position start, Position end)
        {
            return new IfStatement(clauses, start, end);
        }

        public IfClause CreateIfClause(Base condition, List<Base> body, Position start, Position end)
        {
            return new IfClause(condition, body, start, end);
        }

        public ElseifClause CreateElseifClause(Base condition, List<Base> body, Position start, Position end)
        {
            return new ElseifClause(condition, body, start, end);
        }

        public ElseClause CreateElseClause(List<Base> body, Position start, Position end)
        {
            return new ElseClause(body, start, end);
        }

        public WhileStatement CreateWhileStatement(Base condition, List<Base> body, Position start, Position end)
        {
            return new WhileStatement(condition, body, start, end);
        }

        public AssignmentStatement CreateAssignmentStatement(Base variable, Base init, Position start, Position end)
        {
            return new AssignmentStatement(variable, init, start, end);
        }

        public CallStatement CreateCallStatement(Base expression, Position start, Position end)
        {
            return new CallStatement(expression, start, end);
        }

        public FunctionDeclaration CreateFunctionDeclaration(string name, List<Base> parameters, List<Base> body, Position start, Position end)
        {
            return new FunctionDeclaration(name, parameters, body, start, end);
        }

        public ForGenericStatement CreateForGenericStatement(Base variable, Base iterator, List<Base> body, Position start, Position end)
        {
            return new ForGenericStatement(variable, iterator, body, start, end);
        }

        public Chunk CreateChunk(List<Base> body, List<string> imports, HashSet<string> namespaces, List<Base> literals, Position start, Position end)
        {
            return new Chunk(body, imports, namespaces, literals, start, end);
        }
        public Identifier CreateIdentifier(string name, Position start, Position end)
        {
            return new Identifier(name, start, end);
        }

        public Literal CreateLiteral(string type, object value, string raw, Position start, Position end)
        {
            return new Literal(type, value, raw, start, end);
        }

        public MemberExpression CreateMemberExpression(Base origin, string indexer, Base identifier, Position start, Position end)
        {
            return new MemberExpression(origin, indexer, identifier, start, end);
        }

        public CallExpression CreateCallExpression(Base origin, List<Base> args, Position start, Position end)
        {
            return new CallExpression(origin, args, start, end);
        }

        public Comment CreateComment(string value, string raw, Position start, Position end)
        {
            return new Comment(value, raw, start, end);
        }

        public UnaryExpression CreateUnaryExpression(string op, Base arg, Position start, Position end)
        {
            if (op == Operator.Not)
            {
                return new UnaryExpression(Type.NegationExpression, null, arg, start, end);
            }
            else if (op == Operator.Plus || op == Operator.Minus)
            {
                return new UnaryExpression(Type.BinaryNegatedExpression, op, arg, start, end);
            }

            return new UnaryExpression(Type.UnaryExpression, op, arg, start, end);
        }

        public MapKeyString CreateMapKeyString(string key, Base value, Position start, Position end)
        {
            return new MapKeyString(key, value, start, end);
        }

        public MapConstructorExpression CreateMapConstructorExpression(List<Base> fields, Position start, Position end)
        {
            return new MapConstructorExpression(fields, start, end);
        }

        public ListValue CreateListValue(Base value, Position start, Position end)
        {
            return new ListValue(value, start, end);
        }

        public ListConstructorExpression CreateListConstructorExpression(List<Base> fields, Position start, Position end)
        {
            return new ListConstructorExpression(fields, start, end);
        }

        public EmptyExpression CreateEmptyExpression(Position start, Position end)
        {
            return new EmptyExpression(start, end);
        }

        public InvalidCodeExpression CreateInvalidCodeExpression(Position start, Position end)
        {
            return new InvalidCodeExpression(start, end);
        }

        public IndexExpression CreateIndexExpression(Base origin, Base index, Position start, Position end)
        {
            return new IndexExpression(origin, index, start, end);
        }

        public EvaluationExpression CreateEvaluationExpression(string op, Base left, Base right, Position start, Position end)
        {
            string type = Type.BinaryExpression;
            if (op == Operator.And || op == Operator.Or) type = Type.LogicalExpression;

            return new EvaluationExpression(type, op, left, right, start, end);
        }

        public ImportExpression CreateImportExpression(string directory, Position start, Position end)
        {
            return new ImportExpression(directory, start, end);
        }

        public DebuggerStatement CreateDebuggerStatement(Position start, Position end)
        {
            return new DebuggerStatement(start, end);
        }
    }
}
