using PseudoScript.Interpreter.Types;
using PseudoScript.Parser;

namespace PseudoScript.Interpreter.Operations
{
    class Continue : Operation
    {
        public readonly new AstProvider.Base item;

        public Continue(AstProvider.Base item) : this(item, null) { }
        public Continue(AstProvider.Base item, string target) : base(null, target)
        {
            this.item = item;
        }

        public override Continue Build(CPSVisit visit)
        {
            return this;
        }

        public override CustomValue Handle(Context ctx)
        {
            if (ctx.loopState != null)
            {
                ctx.loopState.IsContinue = true;
            }
            return Default.Void;
        }
    }
}
