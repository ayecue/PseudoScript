using PseudoScript.Interpreter.Types;
using PseudoScript.Parser;
using System.Collections.Generic;
using System.Linq;

namespace PseudoScript.Interpreter.Operations
{
    class Map : Operation
    {
        public readonly new AstProvider.MapConstructorExpression item;
        public Dictionary<string, Operation> fields;

        public Map(AstProvider.MapConstructorExpression item) : base(item) { }
        public Map(AstProvider.MapConstructorExpression item, string target) : base(null, target)
        {
            this.item = item;
        }

        public override Map Build(CPSVisit visit)
        {
            fields = new();
            item.fields.ForEach((child) =>
            {
                AstProvider.MapKeyString mapKeyString = (AstProvider.MapKeyString)child;
                fields.Add(mapKeyString.key, visit(mapKeyString.value));
            });
            return this;
        }

        public override CustomMap Handle(Context ctx)
        {
            Dictionary<string, CustomValue> newMap = new();
            fields.ToList().ForEach((child) => newMap.Add(child.Key, child.Value.Handle(ctx)));

            return new CustomMap(newMap);
        }
    }
}
