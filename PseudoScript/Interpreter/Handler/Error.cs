using System;

namespace PseudoScript.Interpreter.Handler
{
    abstract public class Error
    {
        abstract public void Raise(Exception err);
    }

    internal class DefaultError : Error
    {
        public override void Raise(Exception err)
        {
            throw err;
        }
    }
}
