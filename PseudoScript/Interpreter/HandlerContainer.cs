namespace PseudoScript.Interpreter
{
    public class HandlerContainer
    {
        public readonly Handler.Resource resourceHandler;
        public readonly Handler.Output outputHandler;
        public readonly Handler.Error errorHandler;

        public HandlerContainer() : this(null, null, null) { }
        public HandlerContainer(Handler.Resource resourceHandler) : this(resourceHandler, null, null) { }
        public HandlerContainer(Handler.Output outputHandler) : this(null, outputHandler, null) { }
        public HandlerContainer(Handler.Error errorHandler) : this(null, null, errorHandler) { }

        public HandlerContainer(Handler.Resource? resourceHandler, Handler.Output? outputHandler, Handler.Error? errorHandler)
        {
            this.resourceHandler = resourceHandler ?? new Handler.DefaultResource();
            this.outputHandler = outputHandler ?? new Handler.DefaultOutput();
            this.errorHandler = errorHandler ?? new Handler.DefaultError();
        }
    }
}
