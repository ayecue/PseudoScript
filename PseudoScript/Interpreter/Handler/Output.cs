using System;

namespace PseudoScript.Interpreter.Handler
{
    abstract public class Output
    {
        abstract public void Print(string message);
    }

    internal class DefaultOutput : Output
    {
        public override void Print(string message)
        {
            Console.WriteLine(message);
        }
    }
}
