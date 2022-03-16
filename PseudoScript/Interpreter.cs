using PseudoScript.Interpreter.CustomTypes;
using PseudoScript.Interpreter.Operations;
using PseudoScript.Parser;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static PseudoScript.Interpreter.Context;

namespace PseudoScript.Interpreter
{
    public class Options
    {
        public string target;
        public Dictionary<string, CustomValue> api;
        public List<string> argv;
        public HandlerContainer handler;
        public Debugger debugger;

        public Options(string? target, Dictionary<string, CustomValue>? api, List<string>? argv, HandlerContainer? handler, Debugger? debugger)
        {
            this.target = target ?? "unknown";
            this.api = api ?? Intrinsics.Init();
            this.argv = argv ?? new List<string>();
            this.handler = handler ?? new HandlerContainer();
            this.debugger = debugger ?? new Debugger();
        }
    }

    public class Interpreter
    {
        string target;
        Dictionary<string, CustomValue> api;
        List<string> argv;
        HandlerContainer handler;
        Debugger debugger;
        Context apiContext;
        Context globalContext;
        CPS cps;

        public Interpreter() : this(new Options(null, null, null, null, null)) { }

        public Interpreter(string target) : this(new Options(target, null, null, null, null)) { }

        public Interpreter(Dictionary<string, CustomValue> api) : this(new Options(null, api, null, null, null)) { }

        public Interpreter(string target, Dictionary<string, CustomValue> api) : this(new Options(target, api, null, null, null)) { }

        public Interpreter(Options options)
        {
            handler = options.handler;
            debugger = options.debugger;
            api = options.api;
            argv = options.argv;

            SetTarget(options.target);
        }

        public Interpreter SetTarget(string target)
        {
            if (apiContext != null && apiContext.IsPending())
            {
                throw new InterpreterException("You cannot set a target while a process is running.");
            }

            this.target = target;

            CPS.Context cpsCtx = new(target, handler);
            cps = new CPS(cpsCtx);

            Context.Options apiCtxOptions = new(target, null, null, null, true, false, debugger, handler, cps, null);
            apiContext = new Context(apiCtxOptions);
            globalContext = apiContext.Fork(Context.Type.Global, Context.State.Default, null, null);

            return this;
        }

        public Interpreter SetDebugger(Debugger debugger)
        {
            if (apiContext != null && apiContext.IsPending())
            {
                throw new InterpreterException("You cannot set a output handler while a process is running.");
            }

            this.debugger = debugger;
            apiContext.debugger = debugger;
            globalContext.debugger = debugger;

            return this;
        }

        public Interpreter SetApi(Dictionary<string, CustomValue> newApi)
        {
            if (apiContext != null && apiContext.IsPending())
            {
                throw new InterpreterException("You cannot set an api while a process is running.");
            }

            api = newApi;

            return this;
        }

        public Interpreter SetHandler(HandlerContainer handler)
        {
            if (apiContext != null && apiContext.IsPending())
            {
                throw new InterpreterException("You cannot set a handler while a process is running.");
            }

            this.handler = handler;
            apiContext.handler = handler;
            globalContext.handler = handler;

            return this;
        }

        public void InjectSynchrounus(string code)
        {
            InjectSynchrounus(code, globalContext);
        }

        public void InjectSynchrounus(string code, Context ctx)
        {
            Operation top = Prepare(code);
            Context injectionCtx = ctx.Fork(Context.Type.Call, Context.State.Temporary, null, true);

            try
            {
                top.Handle(globalContext);
            }
            catch (Exception err)
            {
                handler.errorHandler.Raise(new InterpreterException(err.Message + ": " + err.StackTrace));
            }
        }

        public void InjectSynchrounusInLastContext(string code)
        {
            Context last = apiContext.GetLastActive();

            if (apiContext != null && apiContext.IsPending())
            {
                InjectSynchrounus(code, last);
            }
            else
            {
                new InterpreterException("Unable to inject into last context.");
            }
        }

        public Task Inject(string code)
        {
            return Inject(code, globalContext);
        }

        public Task Inject(string code, Context ctx)
        {
            Task task = new(() =>
            {
                InjectSynchrounus(code, ctx);
            });

            task.Start();

            return task;
        }

        public Task InjectInLastContext(string code)
        {
            Context last = apiContext.GetLastActive();

            if (apiContext != null && apiContext.IsPending())
            {
                return Inject(code, last);
            }

            throw new InterpreterException("Unable to inject into last context.");
        }

        public Operation Prepare(string code)
        {
            try
            {
                Parser.Parser parser = new(code);
                AstProvider.Base chunk = parser.ParseChunk();
                return cps.Visit(chunk);
            }
            catch (Exception err)
            {
                handler.errorHandler.Raise(err);
            }
            finally
            {
                apiContext.SetPending(false);
            }

            return new Noop();
        }

        public Task Run(string code)
        {
            Operation top = Prepare(code);
            return Run(top);
        }

        public Task Run(Operation top)
        {
            Task task = new(() =>
            {
                RunSynchronous(top);
            });

            task.Start();

            return task;
        }

        public Task Run()
        {
            return Run(handler.resourceHandler.Get(target));
        }

        public void RunSynchronous(string code)
        {
            Operation top = Prepare(code);
            RunSynchronous(top);
        }

        public void RunSynchronous(Operation top)
        {
            if (apiContext != null && apiContext.IsPending())
            {
                throw new InterpreterException("Process already running.");
            }

            apiContext.Extend(api);

            CustomList newArgv = new();

            argv.ForEach(item => newArgv.value.Add(new CustomString(item)));

            globalContext.scope.Set("params", newArgv);

            try
            {
                apiContext.SetPending(true);
                top.Handle(globalContext);
            }
            catch (Exception err)
            {
                handler.errorHandler.Raise(new InterpreterException(err.Message + ": " + err.StackTrace));
            }
            finally
            {
                apiContext.SetPending(false);
            }
        }

        public void RunSynchronous()
        {
            RunSynchronous(handler.resourceHandler.Get(target));
        }

        public void Resume()
        {
            if (apiContext != null && apiContext.IsPending())
            {
                debugger.SetBreakpoint(false);
            }
        }

        public void Pause()
        {
            if (apiContext != null && apiContext.IsPending())
            {
                debugger.SetBreakpoint(true);
            }
        }

        public void Exit()
        {
            if (apiContext != null && apiContext.IsPending())
            {
                apiContext.Exit();
            }
        }

        public void SetGlobalVariable(string path, CustomValue value)
        {
            if (globalContext != null)
            {
                globalContext.Set(path, value);
            }
        }

        public CustomValue GetGlobalVariable(string path)
        {
            if (globalContext != null)
            {
                return globalContext.Get(path);
            }

            return CustomNil.Void;
        }
    }
}
