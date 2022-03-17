using PseudoScript.Interpreter.Types;
using PseudoScript.Parser;

namespace PseudoScript.Interpreter.Operations
{
    class Import : Operation
    {
        public readonly new AstProvider.ImportExpression item;
        public string code;
        public AstProvider.Base chunk;
        public Operation top;

        public Import(AstProvider.ImportExpression item, string target, string code) : base(null, target)
        {
            this.item = item;
            this.code = code;
        }

        public override Import Build(CPSVisit visit)
        {
            Parser.Parser parser = new(code);
            chunk = parser.ParseChunk();
            top = visit(chunk);
            return this;
        }

        public override CustomValue Handle(Context ctx)
        {
            Context importCtx = ctx.Fork(Context.Type.External, Context.State.Temporary, target);
            return top.Handle(importCtx);
        }
    }
}
