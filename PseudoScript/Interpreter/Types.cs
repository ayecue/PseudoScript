using PseudoScript.Interpreter.Operations;
using PseudoScript.Interpreter.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PseudoScript.Interpreter.CustomTypes
{
    public abstract class CustomValue
    {
        public abstract string GetCustomType();
        public abstract double ToNumber();
        public abstract int ToInt();
        public abstract string ToString();
        public abstract bool ToTruthy();
        public abstract CustomValue Fork();
    }

    public abstract class CustomValueWithIntrinsics : CustomValue
    {
        public abstract bool Has(Path<string> path);
        public abstract void Set(Path<string> path, CustomValue value);
        public abstract CustomValue Get(Path<string> path);
        public abstract IEnumerator GetEnumerator();

    }

    public abstract class CustomObject : CustomValueWithIntrinsics
    {
    }

    public class CustomNil : CustomValue
    {
        static public readonly CustomNil Void = new();

        public override string GetCustomType()
        {
            return "null";
        }

        public override string ToString()
        {
            return "null";
        }

        public override CustomNil Fork()
        {
            return new CustomNil();
        }

        public override double ToNumber()
        {
            return double.NaN;
        }

        public override int ToInt()
        {
            return 0;
        }

        public override bool ToTruthy()
        {
            return false;
        }
    }

    public class CustomBoolean : CustomValue
    {
        public bool value;

        public CustomBoolean(bool value)
        {
            this.value = value;
        }

        public override string GetCustomType()
        {
            return "boolean";
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public override CustomBoolean Fork()
        {
            return new CustomBoolean(value);
        }

        public override double ToNumber()
        {
            return value ? 1.0 : 0.0;
        }

        public override int ToInt()
        {
            return value ? 1 : 0;
        }

        public override bool ToTruthy()
        {
            return value;
        }
    }

    public class CustomNumber : CustomValue
    {
        public double value;

        public CustomNumber(int value) : this((double)value) { }

        public CustomNumber(double value)
        {
            this.value = value;
        }

        public override string GetCustomType()
        {
            return "number";
        }

        public override string ToString()
        {
            return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        public override CustomNumber Fork()
        {
            return new CustomNumber(value);
        }

        public override int ToInt()
        {
            return (int)value;
        }

        public override double ToNumber()
        {
            return value;
        }

        public override bool ToTruthy()
        {
            return Convert.ToBoolean(value);
        }
    }

    public class CustomString : CustomValueWithIntrinsics
    {
        public class Enumerator : IEnumerator
        {
            readonly string value;
            int index = -1;

            public Enumerator(string value)
            {
                this.value = value;
            }

            object IEnumerator.Current
            {
                get
                {
                    return new CustomString(value[index].ToString());
                }
            }

            bool IEnumerator.MoveNext()
            {
                return index++ < value.Length;
            }

            void IEnumerator.Reset()
            {
                index = -1;
            }
        }

        static public int GetCharIndex(CustomString item, int index)
        {
            int n = index;
            if (n < 0) n += item.value.Length;
            if (n < 0 || n >= item.value.Length) return -1;
            return n;
        }

        public static Dictionary<string, CustomFunction> intrinsics = new();

        static public void AddIntrinsic(string name, CustomFunction fn)
        {
            intrinsics[name] = fn;
        }

        public string value;

        public CustomString(string value)
        {
            this.value = value;
        }

        public override string GetCustomType()
        {
            return "string";
        }

        public override string ToString()
        {
            return value;
        }

        public override CustomString Fork()
        {
            return new CustomString(value);
        }

        public override double ToNumber()
        {
            bool isNumber = double.TryParse(value, out double result);
            return isNumber ? result : 0;
        }

        public override int ToInt()
        {
            bool isNumber = int.TryParse(value, out int result);
            return isNumber ? result : 0;
        }

        public override bool ToTruthy()
        {
            return value.Length > 0;
        }

        public override Enumerator GetEnumerator()
        {
            return new Enumerator(value);
        }

        public int GetCharIndex(int index)
        {
            return GetCharIndex(this, index);
        }

        public bool Has(string path)
        {
            return Has(new Path<string>(path));
        }

        public override bool Has(Path<string> path)
        {
            Path<string> traversalPath = new(path);
            string current = traversalPath.Next();

            if (current != null)
            {
                return int.TryParse(current, out _);
            }

            return false;
        }

        public void Set(string path, CustomValue newValue)
        {
            Set(new Path<string>(path), newValue);
        }

        public override void Set(Path<string> path, CustomValue newValue)
        {
            throw new InterpreterException("Mutable operations are not allowed on string.");
        }

        public CustomValue Get(string path)
        {
            return Get(new Path<string>(path));
        }

        public override CustomValue Get(Path<string> path)
        {
            if (path.Count == 0)
            {
                return this;
            }

            Path<string> traversalPath = new(path);
            string current = traversalPath.Next();

            if (current != null)
            {
                bool isCurrentNumber = int.TryParse(current, out int currentIndex);

                if (isCurrentNumber)
                {
                    currentIndex = GetCharIndex(currentIndex);
                }

                if (isCurrentNumber)
                {
                    return new CustomString(value[currentIndex].ToString());
                }
                else if (path.Count == 1 && CustomString.intrinsics.ContainsKey(current))
                {
                    return CustomString.intrinsics[current];
                }
            }

            return CustomNil.Void;
        }
    }

    public delegate CustomValue CustomFunctionCallback(Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments);

    public class CustomFunction : CustomValue
    {
        public class Argument
        {
            public string name;
            public Operation defaultValue;

            public Argument(string name) : this(name, CustomNil.Void) { }
            public Argument(string name, CustomValue defaultValue) : this(name, new Reference(defaultValue)) { }

            public Argument(string name, Operation defaultValue)
            {
                this.name = name;
                this.defaultValue = defaultValue;
            }
        }

        public Context? scope;
        public string name;
        public CustomFunctionCallback callback;
        public List<Argument> argumentDefs;

        public CustomFunction(CustomFunctionCallback callback) : this(null, "anonymous", callback) { }

        public CustomFunction(string name, CustomFunctionCallback callback) : this(null, name, callback) { }

        public CustomFunction(Context scope, string name, CustomFunctionCallback callback)
        {
            this.scope = scope;
            this.name = name;
            this.callback = callback;
            argumentDefs = new List<Argument>();
        }

        public CustomFunction AddArgument(string name)
        {
            argumentDefs.Add(new Argument(name, CustomNil.Void));
            return this;
        }

        public CustomFunction AddArgument(string name, CustomValue defaultValue)
        {
            argumentDefs.Add(new Argument(name, defaultValue));
            return this;
        }

        public CustomFunction AddArgument(string name, Operation op)
        {
            argumentDefs.Add(new Argument(name, op));
            return this;
        }

        public override CustomFunction Fork()
        {
            return new CustomFunction(scope, name, callback);
        }

        public override string GetCustomType()
        {
            return "function";
        }

        public override double ToNumber()
        {
            return double.NaN;
        }

        public override int ToInt()
        {
            return 0;
        }

        public override string ToString()
        {
            List<string> args = new();
            argumentDefs.ForEach((item) => args.Add(item.name));
            return string.Format("function {0}({1})", name, String.Join(", ", args));
        }

        public override bool ToTruthy()
        {
            return true;
        }

        public CustomValue Run(CustomValue self, List<CustomValue> arguments)
        {
            Context fnCtx = scope?.Fork(Context.Type.Function, Context.State.Default);
            Dictionary<string, CustomValue> argMap = new();

            for (int index = 0; index < argumentDefs.Count; index++)
            {
                Argument item = argumentDefs[index];

                argMap[item.name] = arguments.ElementAtOrDefault(index) ?? item.defaultValue.Handle(fnCtx);
            }

            return callback(fnCtx, self, argMap);
        }
    }

    public class CustomList : CustomObject
    {
        public class Enumerator : IEnumerator
        {
            readonly List<CustomValue> value;
            int index = -1;

            public Enumerator()
            {
                this.value = new List<CustomValue>();
            }

            public Enumerator(List<CustomValue> value)
            {
                this.value = new List<CustomValue>(value);
            }

            object IEnumerator.Current
            {
                get
                {
                    return value[index];
                }
            }

            bool IEnumerator.MoveNext()
            {
                return ++index < value.Count;
            }

            void IEnumerator.Reset()
            {
                index = -1;
            }
        }

        static public int GetItemIndex(CustomList item, int index)
        {
            int n = index;
            if (n < 0) n += item.value.Count;
            if (n < 0 || n >= item.value.Count) return -1;
            return n;
        }

        public static Dictionary<string, CustomFunction> intrinsics = new();

        static public void AddIntrinsic(string name, CustomFunction fn)
        {
            intrinsics[name] = fn;
        }

        public List<CustomValue> value;

        public CustomList() : this(new List<CustomValue>()) { }
        public CustomList(CustomList list) : this(list.value) { }

        public CustomList(List<CustomValue> value)
        {
            this.value = new List<CustomValue>(value);
        }

        public override string GetCustomType()
        {
            return "list";
        }

        public override string ToString()
        {
            List<string> values = new();
            value.ForEach((item) => values.Add(item.ToString()));
            return string.Format("[ {0} ]", String.Join(", ", values));
        }

        public override CustomList Fork()
        {
            return new CustomList(value);
        }

        public override double ToNumber()
        {
            return double.NaN;
        }

        public override int ToInt()
        {
            return 0;
        }

        public override bool ToTruthy()
        {
            return value.Count > 0;
        }

        public CustomList Extend(CustomList list)
        {
            return Extend(list.value);
        }

        public CustomList Extend(List<CustomValue> list)
        {
            value = value.Concat(list).ToList();
            return this;
        }

        public override Enumerator GetEnumerator()
        {
            return new Enumerator(value);
        }

        public int GetItemIndex(int index)
        {
            return GetItemIndex(this, index);
        }

        public bool Has(string path)
        {
            return Has(new Path<string>(path));
        }

        public override bool Has(Path<string> path)
        {
            Path<string> traversalPath = new(path);
            string current = traversalPath.Next();

            if (current != null)
            {
                bool isCurrentNumber = int.TryParse(current, out int currentIndex);

                if (isCurrentNumber)
                {
                    currentIndex = GetItemIndex(currentIndex);
                }

                if (isCurrentNumber)
                {
                    CustomValue sub = value[currentIndex];

                    if (traversalPath.Count > 0 && sub is CustomValueWithIntrinsics)
                    {
                        return ((CustomValueWithIntrinsics)sub).Has(traversalPath);
                    }

                    return traversalPath.Count == 0;
                }
            }

            return false;
        }

        public void Set(string path, CustomValue newValue)
        {
            Set(new Path<string>(path), newValue);
        }

        public override void Set(Path<string> path, CustomValue newValue)
        {
            Path<string> traversalPath = new(path);
            string last = traversalPath.Last();
            string current = traversalPath.Next();

            if (current != null)
            {
                bool isCurrentNumber = int.TryParse(current, out int currentIndex);

                if (isCurrentNumber)
                {
                    currentIndex = GetItemIndex(currentIndex);
                }

                if (isCurrentNumber && 0 <= currentIndex && currentIndex < value.Count)
                {
                    CustomValue sub = value[currentIndex];

                    if (traversalPath.Count > 0 && sub is CustomValueWithIntrinsics)
                    {
                        ((CustomValueWithIntrinsics)sub).Set(traversalPath, newValue);
                        return;
                    }
                }

                throw new InterpreterException(string.Format("Cannot set path {0}.", path));
            }

            bool isLastNumber = int.TryParse(last, out int lastIndex);

            if (isLastNumber)
            {
                lastIndex = GetItemIndex(lastIndex);
            }

            if (isLastNumber && 0 <= lastIndex && lastIndex < value.Count)
            {
                value[lastIndex] = newValue;
                return;
            }

            throw new InterpreterException(string.Format("Index error (list index {0} out of range).", lastIndex));
        }

        public CustomValue Get(string path)
        {
            return Get(new Path<string>(path));
        }

        public override CustomValue Get(Path<string> path)
        {
            if (path.Count == 0)
            {
                return this;
            }

            Path<string> traversalPath = new(path);
            string current = traversalPath.Next();

            if (current != null)
            {
                bool isCurrentNumber = int.TryParse(current, out int currentIndex);

                if (isCurrentNumber)
                {
                    currentIndex = GetItemIndex(currentIndex);
                }

                if (isCurrentNumber && 0 <= currentIndex && currentIndex < value.Count)
                {
                    CustomValue sub = value[currentIndex];

                    if (traversalPath.Count > 0)
                    {
                        if (sub is CustomValueWithIntrinsics)
                        {
                            return ((CustomValueWithIntrinsics)sub).Get(traversalPath);
                        }
                    }
                    else if (traversalPath.Count == 0)
                    {
                        return sub;
                    }
                }
                else if (path.Count == 1 && CustomList.intrinsics.ContainsKey(current))
                {
                    return CustomList.intrinsics[current];
                }
            }

            return CustomNil.Void;
        }
    }

    public class CustomMap : CustomObject
    {
        public class Enumerator : IEnumerator
        {
            readonly Dictionary<string, CustomValue> value;
            readonly List<string> keys;
            int index = -1;

            public Enumerator(Dictionary<string, CustomValue> value)
            {
                this.value = new Dictionary<string, CustomValue>(value);
                this.keys = new List<string>(value.Keys);
            }

            object IEnumerator.Current
            {
                get
                {
                    Dictionary<string, CustomValue> currentValue = new();
                    string key = keys[index];

                    currentValue.Add("key", new CustomString(key));
                    currentValue.Add("value", value[key]);

                    return new CustomMap(currentValue);
                }
            }

            bool IEnumerator.MoveNext()
            {
                return ++index < keys.Count;
            }

            void IEnumerator.Reset()
            {
                index = -1;
            }
        }

        public static Dictionary<string, CustomFunction> intrinsics = new();

        static public void AddIntrinsic(string name, CustomFunction fn)
        {
            intrinsics[name] = fn;
        }

        public Dictionary<string, CustomValue> value;
        public bool isInstance = false;

        public CustomMap() : this(new Dictionary<string, CustomValue>()) { }
        public CustomMap(CustomMap map) : this(map.value) { }

        public CustomMap(Dictionary<string, CustomValue> value)
        {
            this.value = new Dictionary<string, CustomValue>(value);
        }

        public override string GetCustomType()
        {
            if (Has("classID"))
            {
                return Get("classID").ToString();
            }
            return "map";
        }

        public override string ToString()
        {
            List<string> values = new();
            value.ToList().ForEach((item) => values.Add(string.Format("{0}: {1}", item.Key, item.Value.ToString())));
            return string.Format("{0} {1} {2}", "{", String.Join(", ", values), "}");
        }

        public override CustomMap Fork()
        {
            return new CustomMap(value);
        }

        public override double ToNumber()
        {
            return double.NaN;
        }

        public override int ToInt()
        {
            return 0;
        }

        public override bool ToTruthy()
        {
            return value.Count > 0;
        }

        public override Enumerator GetEnumerator()
        {
            return new Enumerator(value);
        }

        public CustomMap Extend(Dictionary<string, CustomValue> value)
        {
            value.ToList().ForEach(item => this.value[item.Key] = item.Value);
            return this;
        }

        public CustomMap Extend(CustomMap map)
        {
            return Extend(map.value);
        }

        public bool Has(string path)
        {
            return Has(new Path<string>(path));
        }

        public override bool Has(Path<string> path)
        {
            Path<string> traversalPath = new(path);
            string current = traversalPath.Next();

            if (current != null)
            {
                if (value.ContainsKey(current))
                {
                    CustomValue sub = value[current];

                    if (traversalPath.Count > 0 && sub is CustomValueWithIntrinsics)
                    {
                        return ((CustomValueWithIntrinsics)sub).Has(traversalPath);
                    }

                    return traversalPath.Count == 0;
                }
            }

            return false;
        }

        public void Set(string path, CustomValue newValue)
        {
            Set(new Path<string>(path), newValue);
        }

        public override void Set(Path<string> path, CustomValue newValue)
        {
            Path<string> traversalPath = new(path);
            string last = traversalPath.Last();
            string current = traversalPath.Next();

            if (current != null)
            {
                if (value.ContainsKey(current))
                {
                    CustomValue sub = value[current];

                    if (sub is CustomValueWithIntrinsics)
                    {
                        ((CustomValueWithIntrinsics)sub).Set(traversalPath, newValue);
                        return;
                    }
                }

                throw new InterpreterException(string.Format("Cannot set path {0}.", path));
            }

            value[last] = newValue;
        }

        public CustomValue Get(string path)
        {
            return Get(new Path<string>(path));
        }

        public override CustomValue Get(Path<string> path)
        {
            if (path.Count == 0)
            {
                return this;
            }

            Path<string> traversalPath = new(path);
            string current = traversalPath.Next();

            if (current != null)
            {
                if (value.ContainsKey(current))
                {
                    CustomValue sub = value[current];

                    if (traversalPath.Count > 0)
                    {
                        if (sub is CustomValueWithIntrinsics)
                        {
                            return ((CustomValueWithIntrinsics)sub).Get(traversalPath);
                        }
                    }
                    else if (traversalPath.Count == 0)
                    {
                        return sub;
                    }
                }
                else if (path.Count == 1 && CustomMap.intrinsics.ContainsKey(current))
                {
                    return CustomMap.intrinsics[current];
                }
            }

            return CustomNil.Void;
        }

        public CustomMap CreateInstance()
        {
            CustomMap newInstance = new(value);
            newInstance.isInstance = true;
            return newInstance;
        }
    }

    public class CustomInterface : CustomObject
    {
        public class Enumerator : IEnumerator
        {
            public Enumerator() { }

            object IEnumerator.Current
            {
                get
                {
                    return CustomNil.Void;
                }
            }

            bool IEnumerator.MoveNext()
            {
                return false;
            }

            void IEnumerator.Reset() { }
        }

        Dictionary<string, CustomFunction> interfaceFns;
        string type;

        public CustomInterface(string type)
        {
            this.type = type;
            interfaceFns = new Dictionary<string, CustomFunction>();
        }

        public override string GetCustomType()
        {
            return type;
        }

        public override string ToString()
        {
            return type;
        }

        public override CustomInterface Fork()
        {
            return this;
        }

        public override double ToNumber()
        {
            return double.NaN;
        }

        public override int ToInt()
        {
            return 0;
        }

        public override bool ToTruthy()
        {
            return true;
        }

        public override IEnumerator GetEnumerator()
        {
            return new Enumerator();
        }

        public bool Has(string path)
        {
            return Has(new Path<string>(path));
        }

        public override bool Has(Path<string> path)
        {
            Path<string> traversalPath = new(path);
            string current = traversalPath.Next();

            if (current != null)
            {
                return interfaceFns.ContainsKey(current);
            }

            return false;
        }

        public void Set(string path, CustomValue newValue)
        {
            Set(new Path<string>(path), newValue);
        }

        public override void Set(Path<string> path, CustomValue newValue)
        {
            throw new InterpreterException("Cannot set property on an interface");
        }

        public CustomValue Get(string path)
        {
            return Get(new Path<string>(path));
        }

        public override CustomValue Get(Path<string> path)
        {
            if (path.Count == 0)
            {
                return this;
            }

            Path<string> traversalPath = new(path);
            string current = traversalPath.Next();

            if (current != null)
            {
                if (interfaceFns.ContainsKey(current))
                {
                    return interfaceFns[current];
                }
            }

            return CustomNil.Void;
        }

        public CustomInterface AddFunction(string name, CustomFunction fn)
        {
            interfaceFns[name] = fn;
            return this;
        }
    }
}
