# Resource handler

By default it uses `System.IO` to import files. Files will get resolved relative to the file it's imported to.

## Create custom resource handler

```
class MyResourceHandler : PseudoScript.Interpreter.ResourceHandler
{
	public override string Get(string target)
    {
        if (File.Exists(target))
        {
            return File.ReadAllText(target);
        }

        return null;
    }

    public override string GetTargetRelativeTo(string source, string target)
    {
        string origin = Path.GetFullPath(Path.Combine(source, ".."));
        string result = Path.GetFullPath(Path.Combine(origin, target));

        if (File.Exists(result))
        {
            return result;
        }

        return null;
    }

    public override bool Has(string target)
    {
        return File.Exists(target);
    }

    public override string Resolve(string target)
    {
        return Path.GetFullPath(target);
    }
}

Interpreter.Interpreter interpreter = new(new Interpreter.Options(null, null, null, new MyResourceHandler(), null));
Task task = interpreter.Run("import \"my-file.src\"");

task.Wait();
```