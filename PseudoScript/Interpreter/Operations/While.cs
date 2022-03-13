using PseudoScript.Interpreter.CustomTypes;
using PseudoScript.Parser;
using System.Collections.Generic;

namespace PseudoScript.Interpreter.Operations
{
    class While : Operation
    {
        public new AstProvider.WhileStatement item;
        public Block block;
        public Operation condition;

        public While(AstProvider.WhileStatement item) : this(item, null) { }
        public While(AstProvider.WhileStatement item, string target) : base(null, target)
        {
            this.item = item;
        }

        public override While Build(CPSVisit visit)
        {
            List<Operation> stack = new();
            item.body.ForEach((child) => stack.Add(visit(child)));
            block = new Block(stack);
            condition = visit(item.condition);
            return this;
        }

        public override CustomValue Handle(Context ctx)
        {
            Context whileCtx = ctx.Fork(Context.Type.Loop, Context.State.Temporary);

            whileCtx.loopState = new Context.LoopState();

            while (condition.Handle(whileCtx).ToTruthy())
            {
                whileCtx.loopState.isContinue = false;
                block.Handle(whileCtx);
                if (whileCtx.loopState.isContinue) continue;
                if (whileCtx.loopState.isBreak || ctx.IsExit()) break;
            }

            return CustomNil.Void;
        }
    }
}
