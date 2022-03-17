using PseudoScript.Interpreter;
using PseudoScript.Interpreter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PseudoScript
{
    public static class Intrinsics
    {
        private static Dictionary<string, CustomValue> GetApi()
        {
            Dictionary<string, CustomValue> apiInterface = new();

            //Generics
            apiInterface.Add(
                "print",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (fnCtx is Context && arguments.TryGetValue("message", out CustomValue value))
                    {
                        fnCtx.handler.outputHandler.Print(value.ToString());
                        return Default.Void;
                    }
                    return Default.Void;
                })
                    .AddArgument("message")
            );

            apiInterface.Add(
                "exit",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (fnCtx is Context && arguments.TryGetValue("message", out CustomValue value))
                    {
                        fnCtx.handler.outputHandler.Print(value.ToString());
                        fnCtx.Exit();
                        return Default.Void;
                    }
                    return Default.Void;
                })
                    .AddArgument("message")
            );

            apiInterface.Add(
                "wait",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("time", out CustomValue value)) Thread.Sleep(value.ToInt());
                    return Default.Void;
                })
                    .AddArgument("time", Default.PositiveOne)
            );

            apiInterface.Add(
                "char",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value))
                    {
                        if (value is CustomNumber)
                        {
                            return new CustomString(char.ConvertFromUtf32(value.ToInt()));
                        }
                    }
                    return Default.Void;
                })
                    .AddArgument("value")
            );

            apiInterface.Add(
                "code",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value))
                    {
                        if (value is CustomString)
                        {
                            return new CustomNumber(char.ConvertToUtf32(value.ToString(), 0));
                        }
                    }
                    return Default.Void;
                })
                    .AddArgument("value")
            );

            apiInterface.Add(
                "toString",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value)) return new CustomString(value.ToString());
                    return Default.Void;
                })
                    .AddArgument("value")
            );

            apiInterface.Add(
                "toNumber",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(value.ToNumber());
                    return Default.Void;
                })
                    .AddArgument("value")
            );

            apiInterface.Add(
                "toInt",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(value.ToInt());
                    return Default.Void;
                })
                    .AddArgument("value")
            );

            apiInterface.Add(
                "toBoolean",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value)) return new CustomBoolean(value.ToTruthy());
                    return Default.Void;
                })
                    .AddArgument("value")
            );

            apiInterface.Add(
                "range",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    CustomValue to = arguments["to"];
                    CustomValue from = arguments["from"];
                    CustomValue step = arguments["step"];

                    if (to == Default.Void)
                    {
                        to = from;
                        from = Default.Zero;
                    }

                    CustomList list = new();

                    for (int index = from.ToInt(); index < to.ToInt(); index += step.ToInt())
                    {
                        list.value.Add(new CustomNumber(index));
                    }

                    return list;
                })
                    .AddArgument("from")
                    .AddArgument("to")
                    .AddArgument("step", Default.PositiveOne)
            );

            Random defaultRandomGenerator = new Random();
            Dictionary<int, Random> randomGeneratorMap = new();

            apiInterface.Add(
                "rng",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    Random generator = defaultRandomGenerator;
                    arguments.TryGetValue("seed", out CustomValue value);

                    if (value != null)
                    {
                        int seed = value.ToInt();

                        if (randomGeneratorMap.ContainsKey(seed))
                        {
                            generator = randomGeneratorMap[seed];
                        }
                        else
                        {
                            generator = new Random(seed);
                            randomGeneratorMap[seed] = generator;
                        }
                    }

                    return new CustomNumber(generator.NextDouble());
                })
                    .AddArgument("seed")
            );

            //math
            apiInterface.Add(
                "abs",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Abs(value.ToNumber()));
                    return Default.Void;
                })
                    .AddArgument("value")
            );

            apiInterface.Add(
               "acos",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Acos(value.ToNumber()));
                   return Default.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "asin",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Asin(value.ToNumber()));
                   return Default.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "atan",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Atan(value.ToNumber()));
                   return Default.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "ceil",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Ceiling(value.ToNumber()));
                   return Default.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "cos",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Cos(value.ToNumber()));
                   return Default.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "floor",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Floor(value.ToNumber()));
                   return Default.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "sin",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Sin(value.ToNumber()));
                   return Default.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "sign",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Sign(value.ToNumber()));
                   return Default.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "round",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Round(value.ToNumber()));
                   return Default.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "sqrt",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Sqrt(value.ToNumber()));
                   return Default.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "pi",
               new CustomNumber(Math.PI)
            );

            return apiInterface;
        }

        static void InitCustomStringIntrinsics()
        {
            CustomString.AddIntrinsic(
                "trim",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (self is CustomString)
                    {
                        return new CustomString(((CustomString)self).value.Trim());
                    }
                    return Default.Void;
                })
            );

            CustomString.AddIntrinsic(
                "toUpperCase",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (self is CustomString)
                    {
                        return new CustomString(((CustomString)self).value.ToUpper());
                    }
                    return Default.Void;
                })
            );

            CustomString.AddIntrinsic(
                "toLowerCase",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (self is CustomString)
                    {
                        return new CustomString(((CustomString)self).value.ToLower());
                    }
                    return Default.Void;
                })
            );

            CustomString.AddIntrinsic(
                "length",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (self is CustomString)
                    {
                        return new CustomNumber(((CustomString)self).value.Length);
                    }
                    return Default.Zero;
                })
            );

            CustomString.AddIntrinsic(
                "indexOf",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value) && self is CustomString)
                    {
                        CustomString selfString = (CustomString)self;
                        int offset = CustomString.GetCharIndex(selfString, arguments["offset"].ToInt());

                        return new CustomNumber(selfString.value.IndexOf(value.ToString(), offset));
                    }
                    return Default.NegativeOne;
                })
                    .AddArgument("value")
                    .AddArgument("offset", Default.Zero)
            );

            CustomString.AddIntrinsic(
                "lastIndexOf",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value) && self is CustomString)
                    {
                        CustomString selfString = (CustomString)self;
                        int offset = CustomString.GetCharIndex(selfString, arguments["offset"].ToInt());

                        return new CustomNumber(selfString.value.LastIndexOf(value.ToString(), offset));
                    }
                    return Default.NegativeOne;
                })
                    .AddArgument("value")
                    .AddArgument("offset", Default.NegativeOne)
            );

            CustomString.AddIntrinsic(
                "contains",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value) && self is CustomString)
                    {
                        return new CustomBoolean(((CustomString)self).value.Contains(value.ToString()));
                    }
                    return Default.False;
                })
                    .AddArgument("value")
            );

            CustomString.AddIntrinsic(
                "replace",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value) && self is CustomString)
                    {
                        return new CustomString(((CustomString)self).value.Replace(value.ToString(), arguments["replaceWith"].ToString()));
                    }
                    return Default.Void;
                })
                    .AddArgument("value")
                    .AddArgument("replaceWith", new CustomString(""))
            );

            CustomString.AddIntrinsic(
                "split",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomString))
                    {
                        return Default.Void;
                    }

                    CustomString selfString = (CustomString)self;
                    string[] items = selfString.value.Split(arguments["delimiter"].ToString());
                    CustomList result = new();

                    for (int index = 0; index < items.Length; index++)
                    {
                        result.value.Add(new CustomString(items[index].ToString()));
                    }

                    return result;
                })
                    .AddArgument("delimiter", new CustomString(","))
            );

            CustomString.AddIntrinsic(
                "slice",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomString))
                    {
                        return Default.Void;
                    }

                    CustomString selfString = (CustomString)self;

                    arguments.TryGetValue("from", out CustomValue from);
                    arguments.TryGetValue("to", out CustomValue to);

                    int start = CustomString.GetCharIndex(selfString, from.ToInt());
                    int end;

                    if (to == Default.Void)
                    {
                        end = selfString.value.Length - 1;
                    }
                    else
                    {
                        end = CustomString.GetCharIndex(selfString, to.ToInt());
                    }

                    int length = selfString.value.Length;

                    if (start < 0 || start >= length)
                    {
                        return Default.Void;
                    }

                    if (end < 0 || end >= length)
                    {
                        return Default.Void;
                    }

                    if (start > end)
                    {
                        return Default.Void;
                    }

                    string sliced = selfString.value.Substring(start, end - start + 1);

                    return new CustomString(sliced);
                })
                    .AddArgument("from", Default.Zero)
                    .AddArgument("to")
            );
        }

        static void InitCustomListIntrinsics()
        {
            CustomList.AddIntrinsic(
                "hasIndex",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value) && self is CustomList)
                    {
                        return new CustomBoolean(((CustomList)self).Has(value.ToString()));
                    }
                    return Default.False;
                })
                    .AddArgument("value")
            );

            CustomList.AddIntrinsic(
                "join",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return Default.Void;
                    }

                    CustomList selfList = (CustomList)self;
                    List<string> stringList = new();

                    foreach (var item in selfList.value)
                    {
                        stringList.Add(item.ToString());
                    }

                    string result = string.Join(arguments["delimiter"].ToString(), stringList);

                    return new CustomString(result);
                })
                    .AddArgument("delimiter", new CustomString(","))
            );

            CustomList.AddIntrinsic(
                "push",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value) && self is CustomList)
                    {
                        ((CustomList)self).value.Add(value);
                        return Default.True;
                    }
                    return Default.False;
                })
                    .AddArgument("value")
            );

            CustomList.AddIntrinsic(
                "unshift",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value) && self is CustomList)
                    {
                        ((CustomList)self).value.Insert(0, value);
                        return Default.True;
                    }
                    return Default.False;
                })
                    .AddArgument("value")
            );

            CustomList.AddIntrinsic(
                "shift",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return Default.Void;
                    }

                    CustomList selfList = (CustomList)self;

                    if (selfList.value.Count > 0)
                    {
                        CustomValue result = selfList.value.First();
                        selfList.value.RemoveAt(0);
                        return result;
                    }


                    return Default.Void;
                })
            );

            CustomList.AddIntrinsic(
                "pop",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return Default.Void;
                    }

                    CustomList selfList = (CustomList)self;

                    if (selfList.value.Count > 0)
                    {
                        CustomValue result = selfList.value.Last();
                        selfList.value.RemoveAt(selfList.value.Count - 1);
                        return result;
                    }


                    return Default.Void;
                })
            );

            CustomList.AddIntrinsic(
               "removeAt",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("index", out CustomValue value) && self is CustomList)
                   {
                       CustomList selfList = (CustomList)self;
                       int index = CustomList.GetItemIndex(selfList, value.ToInt());

                       if (index >= 0 && index < selfList.value.Count)
                       {
                           selfList.value.RemoveAt(index);
                           return Default.True;
                       }
                   }
                   return Default.False;
               })
                    .AddArgument("index")
           );

            CustomList.AddIntrinsic(
                "length",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (self is CustomList)
                    {
                        return new CustomNumber(((CustomList)self).value.Count);
                    }
                    return Default.Zero;
                })
            );

            CustomList.AddIntrinsic(
                "slice",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return Default.Void;
                    }

                    CustomList selfList = (CustomList)self;

                    arguments.TryGetValue("from", out CustomValue from);
                    arguments.TryGetValue("to", out CustomValue to);

                    if (to == Default.Void)
                    {
                        to = new CustomNumber(selfList.value.Count - 1);
                    }

                    int start = CustomList.GetItemIndex(selfList, from.ToInt());
                    int end = CustomList.GetItemIndex(selfList, to.ToInt());
                    int length = selfList.value.Count;

                    if (start < 0 || start >= length)
                    {
                        return Default.Void;
                    }

                    if (end < 0 || end >= length)
                    {
                        return Default.Void;
                    }

                    if (start > end)
                    {
                        return Default.Void;
                    }

                    List<CustomValue> sliced = selfList.value.GetRange(start, end - start + 1);

                    return new CustomList(sliced);
                })
                    .AddArgument("from", Default.Zero)
                    .AddArgument("to")
            );

            CustomList.AddIntrinsic(
                "values",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return Default.Void;
                    }

                    CustomList result = new();
                    CustomList selfList = (CustomList)self;

                    foreach (var item in selfList.value)
                    {
                        result.value.Add(item);
                    }

                    return result;
                })
            );

            CustomList.AddIntrinsic(
                "keys",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return Default.Void;
                    }

                    CustomList result = new();
                    CustomList selfList = (CustomList)self;
                    int index = 0;

                    foreach (var item in selfList.value)
                    {
                        result.value.Add(new CustomNumber(index++));
                    }

                    return result;
                })
            );

            CustomList.AddIntrinsic(
                "indexOf",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value) && self is CustomList)
                    {
                        CustomList selfList = (CustomList)self;
                        int index = CustomList.GetItemIndex(selfList, arguments["offset"].ToInt());

                        for (; index < selfList.value.Count; index++)
                        {
                            var item = selfList.value[index];

                            if (value.ToString() == item.ToString())
                            {
                                return new CustomNumber(index);
                            }
                        }
                    }
                    return Default.NegativeOne;
                })
                    .AddArgument("value")
                    .AddArgument("offset", Default.Zero)
            );

            CustomList.AddIntrinsic(
                "lastIndexOf",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value) && self is CustomList)
                    {
                        CustomList selfList = (CustomList)self;
                        int index = CustomList.GetItemIndex(selfList, arguments["offset"].ToInt());

                        for (; index == 0; --index)
                        {
                            var item = selfList.value[index];

                            if (value.ToString() == item.ToString())
                            {
                                return new CustomNumber(index);
                            }
                        }
                    }
                    return Default.NegativeOne;
                })
                    .AddArgument("value")
                    .AddArgument("offset", Default.NegativeOne)
            );

            CustomList.AddIntrinsic(
                "find",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return Default.Void;
                    }

                    if (arguments.TryGetValue("fn", out CustomValue fn))
                    {
                        if (!(fn is CustomFunction))
                        {
                            return Default.Void;
                        }

                        CustomList selfList = (CustomList)self;
                        CustomFunction fnRef = (CustomFunction)fn;
                        int index = 0;

                        foreach (var item in selfList.value)
                        {
                            CustomValue result = fnRef.Run(self, new List<CustomValue>() { item, new CustomNumber(index++) });

                            if (result.ToTruthy())
                            {
                                return item;
                            }
                        }
                    }
                    return Default.Void;
                })
                    .AddArgument("fn")
            );

            CustomList.AddIntrinsic(
                "forEach",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return Default.Void;
                    }

                    if (arguments.TryGetValue("fn", out CustomValue fn))
                    {
                        if (!(fn is CustomFunction))
                        {
                            return Default.Void;
                        }

                        CustomList selfList = (CustomList)self;
                        CustomFunction fnRef = (CustomFunction)fn;
                        int index = 0;

                        foreach (var item in selfList.value)
                        {
                            fnRef.Run(self, new List<CustomValue>() { item, new CustomNumber(index++) });
                        }
                    }
                    return Default.Void;
                })
                    .AddArgument("fn")
            );

            CustomList.AddIntrinsic(
                "map",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return Default.Void;
                    }

                    if (arguments.TryGetValue("fn", out CustomValue fn))
                    {
                        if (!(fn is CustomFunction))
                        {
                            return Default.Void;
                        }

                        CustomList selfList = (CustomList)self;
                        CustomList result = new();
                        CustomFunction fnRef = (CustomFunction)fn;
                        int index = 0;

                        foreach (var item in selfList.value)
                        {
                            CustomValue mappedValue = fnRef.Run(self, new List<CustomValue>() { item, new CustomNumber(index++) });
                            result.value.Add(mappedValue);
                        }

                        return result;
                    }
                    return Default.Void;
                })
                    .AddArgument("fn")
            );

            CustomList.AddIntrinsic(
                "reduce",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return Default.Void;
                    }

                    if (arguments.TryGetValue("fn", out CustomValue fn))
                    {
                        if (!(fn is CustomFunction))
                        {
                            return Default.Void;
                        }

                        CustomList selfList = (CustomList)self;
                        CustomValue result = arguments["initalValue"];
                        CustomFunction fnRef = (CustomFunction)fn;
                        int index = 0;

                        foreach (var item in selfList.value)
                        {
                            result = fnRef.Run(self, new List<CustomValue>() { result, item, new CustomNumber(index++) });
                        }

                        return result;
                    }
                    return Default.Void;
                })
                    .AddArgument("fn")
                    .AddArgument("initalValue", Default.Void)
            );
        }

        static void InitCustomMapIntrinsics()
        {
            CustomMap.AddIntrinsic(
                "hasIndex",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value) && self is CustomMap)
                    {
                        return new CustomBoolean(((CustomMap)self).Has(value.ToString()));
                    }
                    return Default.False;
                })
                    .AddArgument("value")
            );

            CustomMap.AddIntrinsic(
                "length",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (self is CustomMap)
                    {
                        return new CustomNumber(((CustomMap)self).value.Count);
                    }
                    return Default.Zero;
                })
            );

            CustomMap.AddIntrinsic(
                "delete",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("key", out CustomValue value) && self is CustomMap)
                    {
                        return new CustomBoolean(((CustomMap)self).value.Remove(value.ToString()));
                    }
                    return Default.False;
                })
                    .AddArgument("key")
            );

            CustomMap.AddIntrinsic(
                "values",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomMap))
                    {
                        return Default.Void;
                    }

                    CustomList result = new();
                    CustomMap selfMap = (CustomMap)self;

                    foreach (var item in selfMap.value)
                    {
                        result.value.Add(item.Value);
                    }

                    return result;
                })
            );

            CustomMap.AddIntrinsic(
                "keys",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomMap))
                    {
                        return Default.Void;
                    }

                    CustomList result = new();
                    CustomMap selfMap = (CustomMap)self;

                    foreach (var item in selfMap.value)
                    {
                        result.value.Add(new CustomString(item.Key));
                    }

                    return result;
                })
            );

            CustomMap.AddIntrinsic(
                "indexOf",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomMap))
                    {
                        return Default.Void;
                    }

                    if (arguments.TryGetValue("value", out CustomValue value) && self is CustomMap)
                    {
                        CustomMap selfMap = (CustomMap)self;

                        foreach (var item in selfMap.value)
                        {
                            if (value.ToString() == item.Value.ToString())
                            {
                                return new CustomString(item.Key);
                            }
                        }
                    }
                    return Default.Void;
                })
                    .AddArgument("value")
            );

            CustomMap.AddIntrinsic(
                "find",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomMap))
                    {
                        return Default.Void;
                    }

                    if (arguments.TryGetValue("fn", out CustomValue fn))
                    {
                        if (!(fn is CustomFunction))
                        {
                            return Default.Void;
                        }

                        CustomMap selfMap = (CustomMap)self;
                        CustomFunction fnRef = (CustomFunction)fn;

                        foreach (var item in selfMap.value)
                        {
                            CustomValue result = fnRef.Run(self, new List<CustomValue>() { item.Value, new CustomString(item.Key) });

                            if (result.ToTruthy())
                            {
                                return item.Value;
                            }
                        }
                    }
                    return Default.Void;
                })
                    .AddArgument("fn")
            );

            CustomMap.AddIntrinsic(
                "forEach",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomMap))
                    {
                        return Default.Void;
                    }

                    if (arguments.TryGetValue("fn", out CustomValue fn))
                    {
                        if (!(fn is CustomFunction))
                        {
                            return Default.Void;
                        }

                        CustomMap selfMap = (CustomMap)self;
                        CustomFunction fnRef = (CustomFunction)fn;

                        foreach (var item in selfMap.value)
                        {
                            fnRef.Run(self, new List<CustomValue>() { item.Value, new CustomString(item.Key) });
                        }
                    }
                    return Default.Void;
                })
                    .AddArgument("fn")
            );
        }

        public static Dictionary<string, CustomValue> Init()
        {
            return Init(new Dictionary<string, CustomValue>());
        }

        public static Dictionary<string, CustomValue> Init(Dictionary<string, CustomValue> customApi)
        {
            Dictionary<string, CustomValue> apiInterface = GetApi();
            customApi.ToList().ForEach((item) => apiInterface[item.Key] = item.Value);

            InitCustomStringIntrinsics();
            InitCustomMapIntrinsics();
            InitCustomListIntrinsics();

            return apiInterface;
        }
    }
}
