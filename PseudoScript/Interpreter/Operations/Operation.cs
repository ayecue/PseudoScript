using PseudoScript.Interpreter.CustomTypes;
using PseudoScript.Parser;

namespace PseudoScript.Interpreter.Operations
{
    abstract public class Operation
    {
        public delegate Operation CPSVisit(AstProvider.Base item);
        public AstProvider.Base item;
        public string target;

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
