using PseudoScript.Interpreter.Types;
using PseudoScript.Parser;

namespace PseudoScript.Interpreter.Operations
{
    class Literal : Operation
    {
        public readonly new AstProvider.Literal item;
        public CustomValue value;

        public Literal(AstProvider.Literal item) : base(item) { }
        public Literal(AstProvider.Literal item, string target) : base(null, target)
        {
            this.item = item;
        }

        public override Literal Build(CPSVisit visit)
        {
            value = item.type switch
            {
                AstProvider.Type.BooleanLiteral => new CustomBoolean((bool)item.value),
                AstProvider.Type.StringLiteral => new CustomString((string)item.value),
                AstProvider.Type.NumericLiteral => new CustomNumber((double)item.value),
                AstProvider.Type.NilLiteral => Default.Void,
                _ => throw new InterpreterException("Unexpected literal type."),
            };
            return this;
        }

        public override CustomValue Handle(Context ctx)
        {
            return value;
        }
    }
}
