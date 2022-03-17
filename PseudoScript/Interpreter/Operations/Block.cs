using PseudoScript.Interpreter.Types;
using System.Collections.Generic;

namespace PseudoScript.Interpreter.Operations
{
    class Block : Operation
    {
        public readonly List<Operation> stack;
        public delegate bool IsEOL();

        public Block(List<Operation> stack) : base(null, "block")
        {
            this.stack = stack;
        }

        public override Block Build(CPSVisit visit)
        {
            return this;
        }

        public override CustomValue Handle(Context ctx)
        {
            IsEOL isEOL = () => false;

            if (ctx.type == Context.Type.Loop)
            {
                isEOL = () => ctx.loopState.IsBreak || ctx.loopState.IsContinue;
            }
            else if (ctx.type == Context.Type.Function)
            {
                isEOL = () => ctx.functionState.IsReturn;
            }

            foreach (Operation entity in stack)
            {
                ctx.Step(entity);
                entity.Handle(ctx);

                if (isEOL() || ctx.IsExit())
                {
                    break;
                }
            }

            return Default.Void;
        }
    }
}
