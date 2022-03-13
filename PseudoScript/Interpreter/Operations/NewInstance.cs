using PseudoScript.Interpreter.CustomTypes;
using PseudoScript.Parser;

namespace PseudoScript.Interpreter.Operations
{
    class NewInstance : Operation
    {
        public new AstProvider.UnaryExpression item;
        Operation arg;

        public NewInstance(AstProvider.UnaryExpression item) : this(item, null) { }
        public NewInstance(AstProvider.UnaryExpression item, string target) : base(null, target)
        {
            this.item = item;
        }

        public override NewInstance Build(CPSVisit visit)
        {
            arg = visit(item.arg);
            return this;
        }

        public override CustomValue Handle(Context ctx)
        {
            CustomValue resolvedArg = arg.Handle(ctx);

            if (resolvedArg is CustomMap)
            {
                return ((CustomMap)resolvedArg).CreateInstance();
            }

            return ctx.debugger.Raise("Only maps can be iniated");
        }
    }
}
