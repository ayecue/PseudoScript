using PseudoScript.Interpreter.Types;
using PseudoScript.Parser;

namespace PseudoScript.Interpreter.Operations
{
    abstract public class Operation
    {
        public delegate Operation CPSVisit(AstProvider.Base item);
        public readonly AstProvider.Base item;
        public readonly string target;

        public Operation(AstProvider.Base item) : this(item, null) { }

        public Operation(AstProvider.Base item, string target)
        {
            this.item = item;
            this.target = target;
        }

        public abstract Operation Build(CPSVisit visit);


        public abstract CustomValue Handle(Context ctx);
    }
}
