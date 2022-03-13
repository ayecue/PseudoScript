using PseudoScript.Interpreter.CustomTypes;

namespace PseudoScript.Interpreter.Operations
{
    class Reference : Operation
    {
        public CustomValue value;

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
