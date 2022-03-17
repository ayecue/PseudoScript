using PseudoScript.Interpreter.Types;

namespace PseudoScript.Interpreter.Operations
{
    class Reference : Operation
    {
        public readonly CustomValue value;

        public Reference(CustomValue value) : base(null, "native")
        {
            this.value = value;
        }

        public override Reference Build(CPSVisit visit)
        {
            return this;
        }

        public override CustomValue Handle(Context ctx)
        {
            return value;
        }
    }
}
