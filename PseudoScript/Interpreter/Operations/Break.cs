using PseudoScript.Interpreter.CustomTypes;
using PseudoScript.Parser;

namespace PseudoScript.Interpreter.Operations
{
    class Break : Operation
    {
        public new AstProvider.Base item;

        public Break(AstProvider.Base item) : this(item, null) { }
        public Break(AstProvider.Base item, string target) : base(null, target)
        {
            this.item = item;
        }

        public override Break Build(CPSVisit visit)
        {
            return this;
        }

        public override CustomValue Handle(Context ctx)
        {
            if (ctx.loopState == null)
            {
                return ctx.debugger.Raise("Unexpected break statement.");
            }

            ctx.loopState.isBreak = true;

            return CustomNil.Void;
        }
    }
}
