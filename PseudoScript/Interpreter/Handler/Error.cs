using System;

namespace PseudoScript.Interpreter.Handler
{
    abstract public class Error
    {
        abstract public void Raise(Exception err);
    }

    public class DefaultError : Error
    {
        public override void Raise(Exception err)
        {
            throw err;
        }
    }
}
