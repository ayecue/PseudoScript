using PseudoScript.Interpreter.CustomTypes;
using PseudoScript.Parser;

namespace PseudoScript.Interpreter.Operations
{
    class DebuggerStatement : Operation
    {
        public new AstProvider.Base item;

        public DebuggerStatement(AstProvider.Base item) : this(item, null) { }
        public DebuggerStatement(AstProvider.Base item, string target) : base(null, target)
        {
            this.item = item;
        }

        public override DebuggerStatement Build(CPSVisit visit)
        {
            return this;
        }

        public override CustomValue Handle(Context ctx)
        {
            ctx.debugger.SetBreakpoint(true);
            return CustomNil.Void;
        }
    }
}
