using PseudoScript.Interpreter.Types;
using PseudoScript.Parser;

namespace PseudoScript.Interpreter.Operations
{
    class Not : Operation
    {
        public readonly new AstProvider.UnaryExpression item;
        public Operation arg;

        public Not(AstProvider.UnaryExpression item) : this(item, null) { }
        public Not(AstProvider.UnaryExpression item, string target) : base(null, target)
        {
            this.item = item;
        }

        public override Not Build(CPSVisit visit)
        {
            arg = visit(item.arg);
            return this;
        }

        public override CustomValue Handle(Context ctx)
        {
            return new CustomBoolean(!arg.Handle(ctx).ToTruthy());
        }
    }
}
