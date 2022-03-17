using PseudoScript.Interpreter.Types;
using PseudoScript.Parser;
using System.Collections.Generic;

namespace PseudoScript.Interpreter.Operations
{
    class List : Operation
    {
        public readonly new AstProvider.ListConstructorExpression item;
        public List<Operation> fields;

        public List(AstProvider.ListConstructorExpression item) : base(item) { }
        public List(AstProvider.ListConstructorExpression item, string target) : base(null, target)
        {
            this.item = item;
        }

        public override List Build(CPSVisit visit)
        {
            fields = new();
            item.fields.ForEach((child) =>
            {
                AstProvider.ListValue listValue = (AstProvider.ListValue)child;
                fields.Add(visit(listValue.value));
            });
            return this;
        }

        public override CustomList Handle(Context ctx)
        {
            List<CustomValue> newList = new();
            fields.ForEach((child) => newList.Add(child.Handle(ctx)));

            return new CustomList(newList);
        }
    }
}
