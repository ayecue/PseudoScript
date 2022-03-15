# Quick start

## Install

Build `PseudoScript.dll` with Visual Studio and add as reference to your project.

## Run script

```
Interpreter.Interpreter interpreter = new();
Task task = interpreter.Run("for i in range(0,10000000);var = i * 100;end for");

task.Wait();
```