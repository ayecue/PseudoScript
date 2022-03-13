using PseudoScript.Interpreter.CustomTypes;
using PseudoScript.Parser;
using System.Collections.Generic;

namespace PseudoScript.Interpreter.Operations
{
    class Call : Operation
    {
        public new AstProvider.CallExpression item;
        public Resolve fnRef;
        public List<Operation> arguments;

        public Call(AstProvider.CallExpression item) : this(item, null) { }
        public Call(AstProvider.CallExpression item, string target) : base(null, target)
        {
            this.item = item;
        }

        public override Call Build(CPSVisit visit)
        {
            fnRef = new Resolve(item.origin).Build(visit);
            arguments = new List<Operation>();
            item.arguments.ForEach((arg) => arguments.Add(visit(arg)));
            return this;
        }

        public override CustomValue Handle(Context ctx)
        {
            Resolve.Result resolveResult = fnRef.GetResult(ctx);
            CustomValue valueRef = fnRef.Handle(ctx, resolveResult);
            List<CustomValue> fnArgs = new();

            arguments.ForEach((arg) => fnArgs.Add(arg.Handle(ctx)));

            if (valueRef is CustomFunction)
            {
                CustomFunction func = (CustomFunction)valueRef;
                return func.Run(resolveResult.handle, fnArgs);
            }

            return ctx.debugger.Raise("Unexpected handle for function call.");
        }
    }
}
