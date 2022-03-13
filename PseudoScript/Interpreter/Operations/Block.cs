using PseudoScript.Interpreter.CustomTypes;
using System.Collections.Generic;

namespace PseudoScript.Interpreter.Operations
{
    class Block : Operation
    {
        public List<Operation> stack;
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
                isEOL = () => ctx.loopState.isBreak || ctx.loopState.isContinue;
            }
            else if (ctx.type == Context.Type.Function)
            {
                isEOL = () => ctx.functionState.isReturn;
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

            return CustomNil.Void;
        }
    }
}
