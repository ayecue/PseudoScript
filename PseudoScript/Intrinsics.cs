using PseudoScript.Interpreter;
using PseudoScript.Interpreter.CustomTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PseudoScript
{
    public static class Intrinsics
    {
        static Dictionary<string, CustomValue> GetApi()
        {
            Dictionary<string, CustomValue> apiInterface = new();

            //Generics
            apiInterface.Add(
                "print",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("message", out CustomValue value)) Console.WriteLine(value.ToString());
                    return CustomNil.Void;
                })
                    .AddArgument("message")
            );

            apiInterface.Add(
                "wait",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("time", out CustomValue value)) Thread.Sleep(value.ToInt());
                    return CustomNil.Void;
                })
                    .AddArgument("time", new CustomNumber(1))
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
                    return CustomNil.Void;
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
                    return CustomNil.Void;
                })
                    .AddArgument("value")
            );

            apiInterface.Add(
                "str",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value)) return new CustomString(value.ToString());
                    return CustomNil.Void;
                })
                    .AddArgument("value")
            );

            apiInterface.Add(
                "val",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(value.ToNumber());
                    return CustomNil.Void;
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

                    if (to == CustomNil.Void)
                    {
                        to = from;
                        from = new CustomNumber(0);
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
                    .AddArgument("step", new CustomNumber(1))
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
                        int seed = value is CustomString ? value.ToString().GetHashCode() : value.ToInt();

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
                    return CustomNil.Void;
                })
                    .AddArgument("value")
            );

            apiInterface.Add(
               "acos",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Acos(value.ToNumber()));
                   return CustomNil.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "asin",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Asin(value.ToNumber()));
                   return CustomNil.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "atan",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Atan(value.ToNumber()));
                   return CustomNil.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "ceil",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Ceiling(value.ToNumber()));
                   return CustomNil.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "cos",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Cos(value.ToNumber()));
                   return CustomNil.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "floor",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Floor(value.ToNumber()));
                   return CustomNil.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "sin",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Sin(value.ToNumber()));
                   return CustomNil.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "sign",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Sign(value.ToNumber()));
                   return CustomNil.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "round",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Round(value.ToNumber()));
                   return CustomNil.Void;
               })
                   .AddArgument("value")
            );

            apiInterface.Add(
               "sqrt",
               new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
               {
                   if (arguments.TryGetValue("value", out CustomValue value)) return new CustomNumber(Math.Sqrt(value.ToNumber()));
                   return CustomNil.Void;
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
                    return CustomNil.Void;
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
                    return CustomNil.Void;
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
                    return CustomNil.Void;
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
                    return new CustomNumber(0);
                })
            );

            CustomString.AddIntrinsic(
                "indexOf",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value) && self is CustomString)
                    {
                        return new CustomNumber(((CustomString)self).value.IndexOf(value.ToString()));
                    }
                    return new CustomNumber(-1);
                })
                    .AddArgument("value")
            );

            CustomString.AddIntrinsic(
                "contains",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value) && self is CustomString)
                    {
                        return new CustomBoolean(((CustomString)self).value.Contains(value.ToString()));
                    }
                    return new CustomBoolean(false);
                })
                    .AddArgument("value")
            );

            CustomString.AddIntrinsic(
                "lastIndexOf",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (arguments.TryGetValue("value", out CustomValue value) && self is CustomString)
                    {
                        return new CustomNumber(((CustomString)self).value.LastIndexOf(value.ToString()));
                    }
                    return new CustomNumber(-1);
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
                    return CustomNil.Void;
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
                        return CustomNil.Void;
                    }

                    CustomString selfString = (CustomString)self;
                    string[] items = selfString.value.Split(arguments["delimiter"].ToString());
                    CustomList result = new();

                    for (int index = 0; index < items.Count(); index++)
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
                        return CustomNil.Void;
                    }

                    CustomString selfString = (CustomString)self;

                    arguments.TryGetValue("from", out CustomValue from);
                    arguments.TryGetValue("to", out CustomValue to);

                    int start = CustomString.GetCharIndex(selfString, from.ToInt());
                    int end;

                    if (to == CustomNil.Void)
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
                        return CustomNil.Void;
                    }

                    if (end < 0 || end >= length)
                    {
                        return CustomNil.Void;
                    }

                    if (start > end)
                    {
                        return CustomNil.Void;
                    }

                    string sliced = selfString.value.Substring(start, end - start + 1);

                    return new CustomString(sliced);
                })
                    .AddArgument("from", new CustomNumber(0))
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
                    return new CustomBoolean(false);
                })
                    .AddArgument("value")
            );

            CustomList.AddIntrinsic(
                "join",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return CustomNil.Void;
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
                        return new CustomBoolean(true);
                    }
                    return new CustomBoolean(false);
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
                        return new CustomBoolean(true);
                    }
                    return new CustomBoolean(false);
                })
                    .AddArgument("value")
            );

            CustomList.AddIntrinsic(
                "shift",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return CustomNil.Void;
                    }

                    CustomList selfList = (CustomList)self;

                    if (selfList.value.Count > 0)
                    {
                        CustomValue result = selfList.value.First();
                        selfList.value.RemoveAt(0);
                        return result;
                    }


                    return CustomNil.Void;
                })
            );

            CustomList.AddIntrinsic(
                "pop",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return CustomNil.Void;
                    }

                    CustomList selfList = (CustomList)self;

                    if (selfList.value.Count > 0)
                    {
                        CustomValue result = selfList.value.Last();
                        selfList.value.RemoveAt(selfList.value.Count - 1);
                        return result;
                    }


                    return CustomNil.Void;
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
                           return new CustomBoolean(true);
                       }
                   }
                   return new CustomBoolean(false);
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
                    return new CustomNumber(0);
                })
            );

            CustomList.AddIntrinsic(
                "slice",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return CustomNil.Void;
                    }

                    CustomList selfList = (CustomList)self;

                    arguments.TryGetValue("from", out CustomValue from);
                    arguments.TryGetValue("to", out CustomValue to);

                    if (to == CustomNil.Void)
                    {
                        to = new CustomNumber(selfList.value.Count - 1);
                    }

                    int start = CustomList.GetItemIndex(selfList, from.ToInt());
                    int end = CustomList.GetItemIndex(selfList, to.ToInt());
                    int length = selfList.value.Count;

                    if (start < 0 || start >= length)
                    {
                        return CustomNil.Void;
                    }

                    if (end < 0 || end >= length)
                    {
                        return CustomNil.Void;
                    }

                    if (start > end)
                    {
                        return CustomNil.Void;
                    }

                    List<CustomValue> sliced = selfList.value.GetRange(start, end - start + 1);

                    return new CustomList(sliced);
                })
                    .AddArgument("from", new CustomNumber(0))
                    .AddArgument("to")
            );

            CustomList.AddIntrinsic(
                "values",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return CustomNil.Void;
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
                        return CustomNil.Void;
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
                        int index = 0;

                        foreach (var item in selfList.value)
                        {
                            if (value.ToString() == item.ToString())
                            {
                                return new CustomNumber(index);
                            }

                            index++;
                        }
                    }
                    return new CustomNumber(-1);
                })
                    .AddArgument("value")
            );

            CustomList.AddIntrinsic(
                "find",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return CustomNil.Void;
                    }

                    if (arguments.TryGetValue("fn", out CustomValue fn))
                    {
                        if (!(fn is CustomFunction))
                        {
                            return CustomNil.Void;
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
                    return CustomNil.Void;
                })
                    .AddArgument("fn")
            );

            CustomList.AddIntrinsic(
                "forEach",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return CustomNil.Void;
                    }

                    if (arguments.TryGetValue("fn", out CustomValue fn))
                    {
                        if (!(fn is CustomFunction))
                        {
                            return CustomNil.Void;
                        }

                        CustomList selfList = (CustomList)self;
                        CustomFunction fnRef = (CustomFunction)fn;
                        int index = 0;

                        foreach (var item in selfList.value)
                        {
                            fnRef.Run(self, new List<CustomValue>() { item, new CustomNumber(index++) });
                        }
                    }
                    return CustomNil.Void;
                })
                    .AddArgument("fn")
            );

            CustomList.AddIntrinsic(
                "map",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return CustomNil.Void;
                    }

                    if (arguments.TryGetValue("fn", out CustomValue fn))
                    {
                        if (!(fn is CustomFunction))
                        {
                            return CustomNil.Void;
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
                    return CustomNil.Void;
                })
                    .AddArgument("fn")
            );

            CustomList.AddIntrinsic(
                "reduce",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomList))
                    {
                        return CustomNil.Void;
                    }

                    if (arguments.TryGetValue("fn", out CustomValue fn))
                    {
                        if (!(fn is CustomFunction))
                        {
                            return CustomNil.Void;
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
                    return CustomNil.Void;
                })
                    .AddArgument("fn")
                    .AddArgument("initalValue", CustomNil.Void)
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
                    return new CustomBoolean(false);
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
                    return new CustomNumber(0);
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
                    return new CustomBoolean(false);
                })
                    .AddArgument("key")
            );

            CustomMap.AddIntrinsic(
                "values",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomMap))
                    {
                        return CustomNil.Void;
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
                        return CustomNil.Void;
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
                        return CustomNil.Void;
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
                    return CustomNil.Void;
                })
                    .AddArgument("value")
            );

            CustomMap.AddIntrinsic(
                "find",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomMap))
                    {
                        return CustomNil.Void;
                    }

                    if (arguments.TryGetValue("fn", out CustomValue fn))
                    {
                        if (!(fn is CustomFunction))
                        {
                            return CustomNil.Void;
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
                    return CustomNil.Void;
                })
                    .AddArgument("fn")
            );

            CustomMap.AddIntrinsic(
                "forEach",
                new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
                {
                    if (!(self is CustomMap))
                    {
                        return CustomNil.Void;
                    }

                    if (arguments.TryGetValue("fn", out CustomValue fn))
                    {
                        if (!(fn is CustomFunction))
                        {
                            return CustomNil.Void;
                        }

                        CustomMap selfMap = (CustomMap)self;
                        CustomFunction fnRef = (CustomFunction)fn;

                        foreach (var item in selfMap.value)
                        {
                            fnRef.Run(self, new List<CustomValue>() { item.Value, new CustomString(item.Key) });
                        }
                    }
                    return CustomNil.Void;
                })
                    .AddArgument("fn")
            );
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
