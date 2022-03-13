using PseudoScript.Interpreter.CustomTypes;
using PseudoScript.Parser;

namespace PseudoScript.Interpreter.Operations
{
    class NegatedBinary : Operation
    {
        public new AstProvider.UnaryExpression item;
        Operation arg;

        public NegatedBinary(AstProvider.UnaryExpression item) : this(item, null) { }
        public NegatedBinary(AstProvider.UnaryExpression item, string target) : base(null, target)
        {
            this.item = item;
        }

        public override NegatedBinary Build(CPSVisit visit)
        {
            arg = visit(item.arg);
            return this;
        }

        public override CustomValue Handle(Context ctx)
        {
            switch (item.op)
            {
                case "-":
                    return new CustomNumber(-arg.Handle(ctx).ToNumber());
                case "+":
                    return new CustomNumber(+arg.Handle(ctx).ToNumber());
                default:
                    throw new InterpreterException("Unexpected negation operator.");
            }
        }
    }
}
