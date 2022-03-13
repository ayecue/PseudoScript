using PseudoScript.Interpreter.CustomTypes;
using PseudoScript.Parser;
using System.Collections.Generic;

namespace PseudoScript.Interpreter.Operations
{
    class Chunk : Operation
    {
        public new AstProvider.Chunk item;
        public Block block;

        public Chunk(AstProvider.Chunk item) : this(item, null) { }
        public Chunk(AstProvider.Chunk item, string target) : base(null, target)
        {
            this.item = item;
        }

        public override Chunk Build(CPSVisit visit)
        {
            List<Operation> stack = new();
            item.body.ForEach((child) => stack.Add(visit(child)));
            block = new Block(stack);
            return this;
        }

        public override CustomValue Handle(Context ctx)
        {
            return block.Handle(ctx);
        }
    }
}
