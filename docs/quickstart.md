# Quick start

## Install

Visit [nuget.org/packages/PseudoScript/](https://www.nuget.org/packages/PseudoScript/).

## Run script

```
Interpreter.Interpreter interpreter = new();
Task task = interpreter.Run("for i in range(0,10000000);var = i * 100;end for");

task.Wait();
```