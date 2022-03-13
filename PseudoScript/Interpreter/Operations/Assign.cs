using PseudoScript.Interpreter.CustomTypes;
using PseudoScript.Parser;

namespace PseudoScript.Interpreter.Operations
{
    class Assign : Operation
    {
        public new AstProvider.AssignmentStatement item;
        public Resolve left;
        public Operation right;

        public Assign(AstProvider.AssignmentStatement item) : this(item, null) { }
        public Assign(AstProvider.AssignmentStatement item, string target) : base(null, target)
        {
            this.item = item;
        }

        public override Assign Build(CPSVisit visit)
        {
            left = new Resolve(item.variable).Build(visit);
            right = visit(item.init);
            return this;
        }

        public override CustomValue Handle(Context ctx)
        {
            Resolve.Result resolveResult = left.GetResult(ctx);
            CustomValue rightValue = right.Handle(ctx);

            if (resolveResult.handle != CustomNil.Void)
            {
                CustomValueWithIntrinsics resultValueCtx = (CustomValueWithIntrinsics)resolveResult.handle;
                resultValueCtx.Set(resolveResult.path, rightValue);
            }
            else
            {
                ctx.Set(resolveResult.path, rightValue);
            }

            return CustomNil.Void;
        }
    }
}
