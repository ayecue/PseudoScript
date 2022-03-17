using PseudoScript.Interpreter.Types;
using PseudoScript.Parser;

namespace PseudoScript.Interpreter.Operations
{
    class Evaluate : Operation
    {
        static class Processor
        {
            static CustomValue HandleNumber(string op, CustomValue a, CustomValue b)
            {
                return op switch
                {
                    Operator.Plus => new CustomNumber(a.ToNumber() + b.ToNumber()),
                    Operator.Minus => new CustomNumber(a.ToNumber() - b.ToNumber()),
                    Operator.Slash => new CustomNumber(a.ToNumber() / b.ToNumber()),
                    Operator.Asterik => new CustomNumber(a.ToNumber() * b.ToNumber()),
                    Operator.Xor => new CustomNumber(a.ToInt() ^ b.ToInt()),
                    Operator.BitwiseOr => new CustomNumber(a.ToInt() | b.ToInt()),
                    Operator.LessThan => new CustomBoolean(a.ToNumber() < b.ToNumber()),
                    Operator.GreaterThan => new CustomBoolean(a.ToNumber() > b.ToNumber()),
                    Operator.LeftShift => new CustomNumber(a.ToInt() << b.ToInt()),
                    Operator.RightShift => new CustomNumber(a.ToInt() >> b.ToInt()),
                    Operator.UnsignedRightShift => new CustomNumber((uint)a.ToInt() >> b.ToInt()),
                    Operator.BitwiseAnd => new CustomNumber(a.ToInt() & b.ToInt()),
                    Operator.PercentSign => new CustomNumber(a.ToNumber() % b.ToNumber()),
                    Operator.GreaterThanOrEqual => new CustomBoolean(a.ToNumber() >= b.ToNumber()),
                    Operator.Equal => new CustomBoolean(a.ToNumber() == b.ToNumber()),
                    Operator.LessThanOrEqual => new CustomBoolean(a.ToNumber() <= b.ToNumber()),
                    Operator.NotEqual => new CustomBoolean(a.ToNumber() != b.ToNumber()),
                    Operator.And => new CustomBoolean(a.ToTruthy() && b.ToTruthy()),
                    Operator.Or => new CustomBoolean(a.ToTruthy() || b.ToTruthy()),
                    _ => Default.Void,
                };
            }

            static CustomValue HandleString(string op, CustomValue a, CustomValue b)
            {
                return op switch
                {
                    Operator.Plus => new CustomString(a.ToString() + b.ToString()),
                    Operator.LessThan => new CustomBoolean(a.ToString().Length < b.ToString().Length),
                    Operator.GreaterThan => new CustomBoolean(a.ToString().Length > b.ToString().Length),
                    Operator.GreaterThanOrEqual => new CustomBoolean(a.ToString().Length >= b.ToString().Length),
                    Operator.Equal => new CustomBoolean(a.ToString() == b.ToString()),
                    Operator.LessThanOrEqual => new CustomBoolean(a.ToString().Length <= b.ToString().Length),
                    Operator.NotEqual => new CustomBoolean(a.ToString() != b.ToString()),
                    Operator.And => new CustomBoolean(a.ToTruthy() && b.ToTruthy()),
                    Operator.Or => new CustomBoolean(a.ToTruthy() || b.ToTruthy()),
                    _ => Default.Void,
                };
            }

            static CustomValue HandleBoolean(string op, CustomValue a, CustomValue b)
            {
                return HandleNumber(op, a, b);
            }

            static CustomValue HandleList(string op, CustomValue a, CustomValue b)
            {
                CustomList left = (CustomList)a;
                CustomList right = b is CustomList ? (CustomList)b : new CustomList();

                return op switch
                {
                    Operator.Plus => new CustomList(left).Extend(right),
                    Operator.LessThan => new CustomBoolean(left.value.Count < right.value.Count),
                    Operator.GreaterThan => new CustomBoolean(left.value.Count > right.value.Count),
                    Operator.GreaterThanOrEqual => new CustomBoolean(left.value.Count >= right.value.Count),
                    Operator.Equal => new CustomBoolean(left.value == right.value),
                    Operator.LessThanOrEqual => new CustomBoolean(left.value.Count <= right.value.Count),
                    Operator.NotEqual => new CustomBoolean(left.value != right.value),
                    Operator.And => new CustomBoolean(a.ToTruthy() && b.ToTruthy()),
                    Operator.Or => new CustomBoolean(a.ToTruthy() || b.ToTruthy()),
                    _ => Default.Void,
                };
            }

            static CustomValue HandleMap(string op, CustomValue a, CustomValue b)
            {
                CustomMap left = (CustomMap)a;
                CustomMap right = b is CustomMap ? (CustomMap)b : new CustomMap();

                return op switch
                {
                    Operator.Plus => new CustomMap(left).Extend(right),
                    Operator.LessThan => new CustomBoolean(left.value.Count < right.value.Count),
                    Operator.GreaterThan => new CustomBoolean(left.value.Count > right.value.Count),
                    Operator.GreaterThanOrEqual => new CustomBoolean(left.value.Count >= right.value.Count),
                    Operator.Equal => new CustomBoolean(left.value == right.value),
                    Operator.LessThanOrEqual => new CustomBoolean(left.value.Count <= right.value.Count),
                    Operator.NotEqual => new CustomBoolean(left.value != right.value),
                    Operator.And => new CustomBoolean(a.ToTruthy() && b.ToTruthy()),
                    Operator.Or => new CustomBoolean(a.ToTruthy() || b.ToTruthy()),
                    _ => Default.Void,
                };
            }

            public static CustomValue Handle(Context ctx, string op, CustomValue a, CustomValue b)
            {
                if (a is CustomString)
                {
                    return HandleString(op, a, b);
                }
                else if (a is CustomNumber)
                {
                    return HandleNumber(op, a, b);
                }
                else if (a is CustomBoolean)
                {
                    return HandleBoolean(op, a, b);
                }
                else if (a is CustomList)
                {
                    return HandleList(op, a, b);
                }
                else if (a is CustomMap)
                {
                    return HandleMap(op, a, b);
                }
                else if (a is CustomNil)
                {
                    return Default.Void;
                }

                return Default.Void;
            }
        }

        public readonly new AstProvider.EvaluationExpression item;
        public string type;
        public string op;
        public Operation left;
        public Operation right;

        public Evaluate(AstProvider.EvaluationExpression item) : this(item, null) { }
        public Evaluate(AstProvider.EvaluationExpression item, string target) : base(null, target)
        {
            this.item = item;
        }

        public override Evaluate Build(CPSVisit visit)
        {
            type = item.type;
            op = item.op;
            left = visit(item.left);
            right = visit(item.right);
            return this;
        }

        public CustomValue Resolve(Context ctx, Operation op)
        {
            if (op is Evaluate)
            {
                Evaluate expr = (Evaluate)op;

                switch (expr.type)
                {
                    case AstProvider.Type.BinaryExpression:
                        return Processor.Handle(ctx, expr.op, Resolve(ctx, expr.left), Resolve(ctx, expr.right));
                    case AstProvider.Type.LogicalExpression:
                        CustomValue leftResult = Resolve(ctx, expr.left);

                        if (expr.op == Operator.And && !leftResult.ToTruthy())
                        {
                            return new CustomBoolean(false);
                        }
                        else if (expr.op == Operator.Or && leftResult.ToTruthy())
                        {
                            return new CustomBoolean(true);
                        }

                        return Processor.Handle(ctx, expr.op, leftResult, Resolve(ctx, expr.right));
                    default:
                        break;
                }
            }

            return op.Handle(ctx);
        }

        public override CustomValue Handle(Context ctx)
        {
            return Resolve(ctx, this);
        }
    }
}
