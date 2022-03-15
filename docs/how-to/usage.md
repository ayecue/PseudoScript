# Usage

There are different way on how to initialize the interpreter.

## Default

- use file as target

```
Interpreter.Interpreter interpreter = new("code-file.src");
Task task = interpreter.Run();
task.Wait();
```

## Default + API

- use file as target
- use custom API

```
Interpreter.Interpreter interpreter = new("code-file.src", apiInterface);
Task task = interpreter.Run();
task.Wait();
```

## Default + API + ResourceHandler

- use file as target
- use custom API
- use custom resource handler

```
Interpreter.Interpreter interpreter = new(new Interpreter.Options(null, null, null, new MyResourceHandler(), null));
Task task = interpreter.Run();
task.Wait();
```

## Advanced

```
Interpreter.Options options = new(
	"code-file.src", //file to execute
	customApiInterface, //custom intrinsics
	argv, //params field which gets extend to the global context. Type of this field is List<CustomValue>; Specific docs TBD
	new MyResourceHandler(), //custom resource handler
	new MyOutputHandler(), //custom output handler; Specific docs TBD
	new MyDebugger() //implement your own debugger; Specific docs TBD
);
Interpreter.Interpreter interpreter = new(options);
Task task = interpreter.Run();
task.Wait();
```