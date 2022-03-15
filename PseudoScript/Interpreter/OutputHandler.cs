using System;

namespace PseudoScript.Interpreter
{
    abstract public class OutputHandler
    {
        abstract public void Print(string message);
    }

    public class DefaultOutputHandler : OutputHandler
    {
        public override void Print(string message)
        {
            Console.WriteLine(message);
        }
    }
}
