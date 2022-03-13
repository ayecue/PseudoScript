using PseudoScript.Interpreter.CustomTypes;
using PseudoScript.Interpreter.Utils;
using PseudoScript.Parser;
using System.Collections.Generic;
using System.Linq;

namespace PseudoScript.Interpreter.Operations
{
    class Function : Operation
    {
        public new AstProvider.FunctionDeclaration item;
        public Operation block;
        public Dictionary<string, Operation> arguments;

        public Function(AstProvider.FunctionDeclaration item) : base(item) { }
        public Function(AstProvider.FunctionDeclaration item, string target) : base(null, target)
        {
            this.item = item;
        }

        public override Function Build(CPSVisit visit)
        {
            List<Operation> stack = new();
            item.body.ForEach((child) => stack.Add(visit(child)));
            block = new Block(stack);
            arguments = new Dictionary<string, Operation>();
            item.arguments.ForEach((child) =>
            {
                switch (child.type)
                {
                    case AstProvider.Type.AssignmentStatement:
                        AstProvider.AssignmentStatement assignStatement = (AstProvider.AssignmentStatement)child;
                        AstProvider.Identifier assignKey = (AstProvider.Identifier)assignStatement.variable;
                        arguments[assignKey.name] = visit(assignStatement.init);
                        break;
                    case AstProvider.Type.Identifier:
                        AstProvider.Identifier identifierKey = (AstProvider.Identifier)child;
                        arguments[identifierKey.name] = new Reference(CustomNil.Void);
                        break;
                    default:
                        throw new InterpreterException("Unexpected operation in arguments.");
                }
            });
            return this;
        }

        public override CustomValue Handle(Context ctx)
        {
            CustomFunction func = new(ctx, item.name, (Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
            {
                fnCtx.functionState = new Context.FunctionState();

                fnCtx.Set(new Path<string>("self"), self);
                arguments.ToList().ForEach((item) => fnCtx.Set(new Path<string>(item.Key), item.Value));

                block.Handle(fnCtx);

                return fnCtx.functionState.value;
            });

            if (item.name != "anonymous")
            {
                ctx.Set(new Path<string>(item.name), func);
            }

            arguments.ToList().ForEach((child) => func.AddArgument(child.Key, child.Value));

            return func;
        }
    }
}
