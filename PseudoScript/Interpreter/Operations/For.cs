using PseudoScript.Interpreter.Types;
using PseudoScript.Parser;
using System.Collections;
using System.Collections.Generic;

namespace PseudoScript.Interpreter.Operations
{
    class For : Operation
    {
        public readonly new AstProvider.ForGenericStatement item;
        public Block block;
        public Resolve variable;
        public Operation iterator;

        public For(AstProvider.ForGenericStatement item) : this(item, null) { }
        public For(AstProvider.ForGenericStatement item, string target) : base(null, target)
        {
            this.item = item;
        }

        public override For Build(CPSVisit visit)
        {
            List<Operation> stack = new();
            item.body.ForEach((child) => stack.Add(visit(child)));
            block = new Block(stack);
            variable = new Resolve(item.variable).Build(visit);
            iterator = visit(item.iterator);
            return this;
        }

        public override CustomValue Handle(Context ctx)
        {
            Context forCtx = ctx.Fork(Context.Type.Loop, Context.State.Temporary);
            Resolve.Result resolveResult = variable.GetResult(ctx);
            CustomValueWithIntrinsics iteratorValue = (CustomValueWithIntrinsics)iterator.Handle(ctx);

            forCtx.loopState = new Context.LoopState();

            IEnumerator enumerator = iteratorValue.GetEnumerator();

            while (enumerator.MoveNext())
            {
                CustomValue current = (CustomValue)enumerator.Current;
                forCtx.loopState.IsContinue = false;
                forCtx.Set(resolveResult.path, current);
                block.Handle(forCtx);
                if (forCtx.loopState.IsContinue) continue;
                if (forCtx.loopState.IsBreak || ctx.IsExit()) break;
            }

            return Default.Void;
        }
    }
}
