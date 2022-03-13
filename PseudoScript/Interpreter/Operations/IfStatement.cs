using PseudoScript.Interpreter.CustomTypes;
using PseudoScript.Parser;
using System.Collections.Generic;

namespace PseudoScript.Interpreter.Operations
{
    class IfStatement : Operation
    {
        public class Clause
        {
            public readonly Operation condition;
            public readonly Block block;

            public Clause(Operation condition, Block block)
            {
                this.condition = condition;
                this.block = block;
            }
        }

        public new AstProvider.IfStatement item;
        public List<Clause> clauses;

        public IfStatement(AstProvider.IfStatement item) : this(item, null) { }
        public IfStatement(AstProvider.IfStatement item, string target) : base(null, target)
        {
            this.item = item;
        }

        public override IfStatement Build(CPSVisit visit)
        {
            clauses = new();
            item.clauses.ForEach((child) =>
            {
                Operation condition;
                List<Operation> stack;
                Block block;

                switch (child.type)
                {
                    case AstProvider.Type.IfClause:
                        AstProvider.IfClause ifClause = (AstProvider.IfClause)child;
                        condition = visit(ifClause.condition);
                        stack = new();
                        ifClause.body.ForEach((clauseBody) => stack.Add(visit(clauseBody)));
                        block = new(stack);
                        clauses.Add(new Clause(condition, block));
                        break;
                    case AstProvider.Type.ElseifClause:
                        AstProvider.ElseifClause elseifClause = (AstProvider.ElseifClause)child;
                        condition = visit(elseifClause.condition);
                        stack = new();
                        elseifClause.body.ForEach((clauseBody) => stack.Add(visit(clauseBody)));
                        block = new(stack);
                        clauses.Add(new Clause(condition, block));
                        break;
                    case AstProvider.Type.ElseClause:
                        AstProvider.ElseClause elseClause = (AstProvider.ElseClause)child;
                        condition = new Reference(new CustomBoolean(true));
                        stack = new();
                        elseClause.body.ForEach((clauseBody) => stack.Add(visit(clauseBody)));
                        block = new(stack);
                        clauses.Add(new Clause(condition, block));
                        break;
                }
            });

            return this;
        }

        public override CustomValue Handle(Context ctx)
        {
            foreach (Clause clause in clauses)
            {
                if (clause.condition.Handle(ctx).ToTruthy())
                {
                    clause.block.Handle(ctx);
                    break;
                }
            }

            return CustomNil.Void;
        }
    }
}
