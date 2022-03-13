using PseudoScript.Interpreter.Operations;
using PseudoScript.Parser;

namespace PseudoScript.Interpreter
{
    public class CPS
    {
        public class Context
        {
            public readonly string target;
            public readonly ResourceHandler resourceHandler;
            public string currentTarget;

            public Context(string target, ResourceHandler resourceHandler)
            {
                this.target = target;
                this.resourceHandler = resourceHandler;
            }

            public Context(string target, ResourceHandler resourceHandler, string currentTarget)
            {
                this.target = target;
                this.resourceHandler = resourceHandler;
                this.currentTarget = currentTarget;
            }
        }

        readonly Context context;

        public CPS(Context context)
        {
            this.context = context;
        }

        public Operation Visit(AstProvider.Base item)
        {
            switch (item.type)
            {
                case AstProvider.Type.MapConstructorExpression:
                    return new Map((AstProvider.MapConstructorExpression)item, context.target).Build((x) => Visit(x));
                case AstProvider.Type.ListConstructorExpression:
                    return new List((AstProvider.ListConstructorExpression)item, context.target).Build((x) => Visit(x));
                case AstProvider.Type.AssignmentStatement:
                    return new Assign((AstProvider.AssignmentStatement)item, context.target).Build((x) => Visit(x));
                case AstProvider.Type.MemberExpression:
                case AstProvider.Type.Identifier:
                case AstProvider.Type.IndexExpression:
                    return new Resolve(item, context.target).Build((x) => Visit(x));
                case AstProvider.Type.FunctionDeclaration:
                    return new Function((AstProvider.FunctionDeclaration)item, context.target).Build((x) => Visit(x));
                case AstProvider.Type.InvalidCodeExpression:
                    return new Noop(item).Build((x) => Visit(x));
                case AstProvider.Type.WhileStatement:
                    return new While((AstProvider.WhileStatement)item, context.target).Build((x) => Visit(x));
                case AstProvider.Type.ForGenericStatement:
                    return new For((AstProvider.ForGenericStatement)item, context.target).Build((x) => Visit(x));
                case AstProvider.Type.IfStatement:
                    return new IfStatement((AstProvider.IfStatement)item, context.target).Build((x) => Visit(x));
                case AstProvider.Type.ReturnStatement:
                    return new Return((AstProvider.ReturnStatement)item, context.target).Build((x) => Visit(x));
                case AstProvider.Type.BreakStatement:
                    return new Continue((AstProvider.BreakStatement)item, context.target).Build((x) => Visit(x));
                case AstProvider.Type.ContinueStatement:
                    return new Continue((AstProvider.ContinueStatement)item, context.target).Build((x) => Visit(x));
                case AstProvider.Type.CallExpression:
                    return new Call((AstProvider.CallExpression)item, context.target).Build((x) => Visit(x));
                case AstProvider.Type.CallStatement:
                    return Visit(((AstProvider.CallStatement)item).expression);
                case AstProvider.Type.ImportExpression:
                    AstProvider.ImportExpression importExpr = (AstProvider.ImportExpression)item;
                    string target = context.resourceHandler.GetTargetRelativeTo(context.target, importExpr.directory);
                    string code = context.resourceHandler.Get(target);

                    context.currentTarget = target;
                    Import importStatement = new Import(importExpr, target, code).Build((x) => Visit(x));
                    context.currentTarget = context.target;

                    return importStatement;
                case AstProvider.Type.DebuggerStatement:
                    return new DebuggerStatement(item, context.target);
                case AstProvider.Type.BooleanLiteral:
                case AstProvider.Type.StringLiteral:
                case AstProvider.Type.NumericLiteral:
                case AstProvider.Type.NilLiteral:
                    return new Literal(((AstProvider.Literal)item), context.target).Build((x) => Visit(x));
                case AstProvider.Type.EmptyExpression:
                    return new Noop(item, context.target).Build((x) => Visit(x));
                case AstProvider.Type.BinaryExpression:
                case AstProvider.Type.LogicalExpression:
                    return new Evaluate((AstProvider.EvaluationExpression)item, context.target).Build((x) => Visit(x));
                case AstProvider.Type.NegationExpression:
                    return new Not((AstProvider.UnaryExpression)item, context.target).Build((x) => Visit(x));
                case AstProvider.Type.BinaryNegatedExpression:
                    return new NegatedBinary((AstProvider.UnaryExpression)item, context.target).Build((x) => Visit(x));
                case AstProvider.Type.UnaryExpression:
                    AstProvider.UnaryExpression unaryExpr = (AstProvider.UnaryExpression)item;

                    if (unaryExpr.op == "new")
                    {
                        return new NewInstance(unaryExpr).Build((x) => Visit(x));
                    }

                    throw new InterpreterException("Unknown unary expression.");
                case AstProvider.Type.Chunk:
                    return new Chunk((AstProvider.Chunk)item, context.target).Build((x) => Visit(x));
                default:
                    throw new InterpreterException(string.Format("Unexpected AST type {0}.", item.type));
            }
        }
    }
}
