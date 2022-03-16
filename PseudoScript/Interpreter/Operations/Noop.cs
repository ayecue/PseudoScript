using PseudoScript.Interpreter.CustomTypes;
using PseudoScript.Parser;

namespace PseudoScript.Interpreter.Operations
{
    class Noop : Operation
    {
        public Noop() : base(null) { }
        public Noop(AstProvider.Base item) : base(item) { }
        public Noop(AstProvider.Base item, string target) : base(item, target) { }

        public override Noop Build(CPSVisit visit)
        {
            return this;
        }

        public override CustomNil Handle(Context ctx)
        {
            return CustomNil.Void;
        }
    }
}
