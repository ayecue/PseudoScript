﻿using PseudoScript.Interpreter.CustomTypes;
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
        public ResourceHandler resourceHandler;
        public Debugger debugger;

        public Options(string? target, Dictionary<string, CustomValue>? api, List<string>? argv, ResourceHandler? resourceHandler, Debugger? debugger)
        {
            this.target = target ?? "unknown";
            this.api = api ?? new Dictionary<string, CustomValue>();
            this.argv = argv ?? new List<string>();
            this.resourceHandler = resourceHandler ?? new DefaultResourceHandler();
            this.debugger = debugger ?? new Debugger();
        }
    }

    public class Interpreter
    {
        public string target;
        public Dictionary<string, CustomValue> api;
        public List<string> argv;
        public ResourceHandler resourceHandler;
        public Debugger debugger;
        public Context apiContext;
        public Context globalContext;
        public CPS cps;

        public Interpreter(Options options)
        {
            resourceHandler = options.resourceHandler;
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

            CPS.Context cpsCtx = new(target, resourceHandler);
            cps = new CPS(cpsCtx);

            Context.Options apiCtxOptions = new(target, null, null, null, true, false, debugger, cps, null);
            apiContext = new Context(apiCtxOptions);
            globalContext = apiContext.Fork(Context.Type.Global, Context.State.Default, null, null);

            return this;
        }

        public Interpreter SetDebugger(Debugger debugger)
        {
            if (apiContext != null && apiContext.IsPending())
            {
                throw new InterpreterException("You cannot set a debugger while a process is running.");
            }

            this.debugger = debugger;
            apiContext.debugger = debugger;
            globalContext.debugger = debugger;

            return this;
        }

        public Task Inject(string code)
        {
            return Inject(code, globalContext);
        }

        public Task Inject(string code, Context ctx)
        {
            Parser.Parser parser = new(code);
            AstProvider.Base chunk = parser.ParseChunk();
            Operation top = cps.Visit(chunk);
            Context injectionCtx = ctx.Fork(Context.Type.Call, Context.State.Temporary, null, true);

            Task task = new(() =>
            {
                try
                {
                    top.Handle(globalContext);
                }
                catch (Exception err)
                {
                    debugger.Raise(err.Message);
                }
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

        public Task Run(string code)
        {
            if (apiContext != null && apiContext.IsPending())
            {
                throw new InterpreterException("Process already running.");
            }

            Parser.Parser parser = new(code);
            AstProvider.Base chunk = parser.ParseChunk();
            Operation top = cps.Visit(chunk);

            apiContext.Extend(api);

            CustomList newArgv = new();

            argv.ForEach(item => newArgv.value.Add(new CustomString(item)));

            globalContext.scope.Set("params", newArgv);

            Task task = new(() =>
            {
                try
                {
                    apiContext.SetPending(true);
                    top.Handle(globalContext);
                }
                catch (Exception err)
                {
                    debugger.Raise(err.Message);
                }
                finally
                {
                    apiContext.SetPending(false);
                }
            });

            task.Start();

            return task;
        }

        public Task Run()
        {
            return Run(resourceHandler.Get(target));
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
    }
}