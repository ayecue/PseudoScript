using PseudoScript.Interpreter.Types;
using PseudoScript.Parser;

namespace PseudoScript.Interpreter.Operations
{
    class Return : Operation
    {
        public readonly new AstProvider.ReturnStatement item;
        Operation arg;

        public Return(AstProvider.ReturnStatement item) : this(item, null) { }
        public Return(AstProvider.ReturnStatement item, string target) : base(null, target)
        {
            this.item = item;
        }

        public override Return Build(CPSVisit visit)
        {
            arg = visit(item.arg);
            return this;
        }

        public override CustomValue Handle(Context ctx)
        {
            if (ctx.functionState != null)
            {
                ctx.functionState.Value = arg.Handle(ctx);
                ctx.functionState.IsReturn = true;
            }
            return Default.Void;
        }
    }
}
