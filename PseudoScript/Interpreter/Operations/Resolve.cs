using PseudoScript.Interpreter.Types;
using PseudoScript.Interpreter.Utils;
using PseudoScript.Parser;
using System.Collections.Generic;

namespace PseudoScript.Interpreter.Operations
{
    class Resolve : Operation
    {
        public class IdentifierSegment
        {
            public readonly string value;

            public IdentifierSegment(string value)
            {
                this.value = value;
            }
        }

        public class IndexSegment
        {
            public readonly Operation op;

            public IndexSegment(Operation op)
            {
                this.op = op;
            }
        }

        public class OperationSegment
        {
            public readonly Operation op;

            public OperationSegment(Operation op)
            {
                this.op = op;
            }
        }

        public class Result
        {
            public readonly Path<string> path;
            public readonly CustomValue handle;

            public Result(Path<string> path, CustomValue handle)
            {
                this.path = path;
                this.handle = handle;
            }
        }

        public readonly new AstProvider.Base item;
        public List<object> path;

        public Resolve(AstProvider.Base item) : this(item, null) { }
        public Resolve(AstProvider.Base item, string target) : base(null, target)
        {
            this.item = item;
        }

        void BuildProcessor(AstProvider.Base node, CPSVisit visit)
        {
            switch (node.type)
            {
                case AstProvider.Type.MemberExpression:
                    AstProvider.MemberExpression memberExpr = (AstProvider.MemberExpression)node;
                    BuildProcessor(memberExpr.origin, visit);
                    BuildProcessor(memberExpr.identifier, visit);
                    break;
                case AstProvider.Type.IndexExpression:
                    AstProvider.IndexExpression indexExpr = (AstProvider.IndexExpression)node;
                    BuildProcessor(indexExpr.origin, visit);
                    IndexSegment indexSegment = new(visit(indexExpr.index));
                    path.Add(indexSegment);
                    break;
                case AstProvider.Type.Identifier:
                    AstProvider.Identifier identifier = (AstProvider.Identifier)node;
                    IdentifierSegment identifierSegment = new(identifier.name);
                    path.Add(identifierSegment);
                    break;
                default:
                    OperationSegment opSegment = new(visit(node));
                    path.Add(opSegment);
                    break;
            }
        }

        public override Resolve Build(CPSVisit visit)
        {
            path = new List<object>();
            BuildProcessor(item, visit);
            return this;
        }

        public Result GetResult(Context ctx)
        {
            Queue<object> segments = new(path);
            Path<string> traversedPath = new();
            CustomValue handle = Default.Void;

            while (segments.TryDequeue(out object current))
            {
                if (current is OperationSegment)
                {
                    OperationSegment opSegment = (OperationSegment)current;
                    handle = opSegment.op.Handle(ctx);
                }
                else if (current is IdentifierSegment)
                {
                    IdentifierSegment identifierSegment = (IdentifierSegment)current;

                    traversedPath.Add(identifierSegment.value);

                    if (segments.Count == 0)
                    {
                        break;
                    }

                    if (handle != Default.Void)
                    {
                        CustomValueWithIntrinsics customValueCtx = (CustomValueWithIntrinsics)handle;
                        handle = customValueCtx.Get(traversedPath);
                    }
                    else
                    {
                        handle = ctx.Get(traversedPath);
                    }

                    traversedPath = new Path<string>();
                }
                else if (current is IndexSegment)
                {
                    IndexSegment indexSegment = (IndexSegment)current;
                    CustomValue indexValue = indexSegment.op.Handle(ctx);

                    traversedPath.Add(indexValue.ToString());

                    if (segments.Count == 0)
                    {
                        break;
                    }

                    if (handle != Default.Void)
                    {
                        CustomValueWithIntrinsics customValueCtx = (CustomValueWithIntrinsics)handle;
                        handle = customValueCtx.Get(traversedPath);
                    }
                    else
                    {
                        handle = ctx.Get(traversedPath);
                    }

                    traversedPath = new Path<string>();
                }
            }

            return new Result(traversedPath, handle);
        }

        public override CustomValue Handle(Context ctx)
        {
            Result result = GetResult(ctx);
            return Handle(ctx, result);
        }

        public CustomValue Handle(Context ctx, Result result)
        {
            if (result.handle != Default.Void)
            {
                if (result.path.Count == 0)
                {
                    return result.handle;
                }

                CustomValueWithIntrinsics customValueCtx = (CustomValueWithIntrinsics)result.handle;
                return customValueCtx.Get(result.path);
            }

            return ctx.Get(result.path);
        }
    }
}
