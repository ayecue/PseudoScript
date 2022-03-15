# How to extend the API

## Extend method

```
Dictionary<string, CustomValue> apiInterface = new();

apiInterface.Add(
    "print",
    new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
    {
        if (arguments.TryGetValue("message", out CustomValue value)) Console.WriteLine(value.ToString());
        return CustomNil.Void;
    })
        .AddArgument("message")
);

Dictionary<string, CustomValue> apiInterface = Intrinsics.Init(mockedApiInterface);
Interpreter.Interpreter interpreter = new(apiInterface);
interpreter.Run("print(123)").Wait();
```

## Extend interface

```
Dictionary<string, CustomValue> apiInterface = new();

apiInterface.Add(
    "get_test_interface",
    new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
    {
        CustomInterface itr = new("testInterface");

        itr.AddFunction("test", new CustomFunction((Context fnCtx, CustomValue self, Dictionary<string, CustomValue> arguments) =>
        {
            if (arguments.TryGetValue("message", out CustomValue value)) Console.WriteLine("test interface print: " + value.ToString());
            return CustomNil.Void;
        })
            .AddArgument("message")
        );

        return itr;
    })
);

Dictionary<string, CustomValue> apiInterface = Intrinsics.Init(mockedApiInterface);
Interpreter.Interpreter interpreter = new(apiInterface);
interpreter.Run("get_test_interface().test(123)").Wait();
```