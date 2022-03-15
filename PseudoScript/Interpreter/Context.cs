using PseudoScript.Interpreter.CustomTypes;
using PseudoScript.Interpreter.Operations;
using PseudoScript.Interpreter.Utils;
using PseudoScript.Parser;
using System;
using System.Collections.Generic;

namespace PseudoScript.Interpreter
{
    public class Context
    {
        public enum Type
        {
            Api,
            Global,
            Function,
            External,
            Loop,
            Map,
            Call
        }

        public enum State
        {
            Temporary,
            Default
        }

        public class Scope : CustomMap
        {
            readonly Context context;

            public Scope(Context context) : base()
            {
                this.context = context;
            }

            public CustomValue Get(string path)
            {
                return Get(new Path<string>(path));
            }

            public override CustomValue Get(Path<string> path)
            {
                Path<string> traversalPath = new(path);
                string current = traversalPath.Next();

                if (path.Count == 0)
                {
                    return this;
                }

                if (current == "locals" || current == "globals")
                {
                    return context.Get(traversalPath);
                }
                else if (Has(path))
                {
                    return base.Get(path);
                }
                else if (context.api.scope.Has(path))
                {
                    return context.api.scope.Get(path);
                }
                else if (path.Count == 1 && CustomMap.intrinsics.Has(current))
                {
                    return CustomMap.intrinsics.Get(current);
                }
                else if (context.previous != null)
                {
                    return context.previous.Get(path);
                }

                return context.debugger.Raise(string.Format("Cannot get path {0}.", path.ToString()));
            }
        }

        public class Debugger
        {
            public bool breakpoint = false;
            public bool nextStep = false;
            public Context lastContext = null;

            public CustomNil Raise(string message)
            {
                return Raise(new InterpreterException(message));
            }

            public CustomNil Raise(InterpreterException err)
            {
                throw new InterpreterException(err.Message);
            }

            public CustomNil Debug(string message)
            {
                Console.WriteLine(message);
                return CustomNil.Void;
            }

            public Debugger SetBreakpoint(bool breakpoint)
            {
                this.breakpoint = breakpoint;
                return this;
            }

            public bool GetBreakpoint(Context ctx)
            {
                return breakpoint;
            }

            public Debugger Next()
            {
                nextStep = true;
                return this;
            }

            public void Resume()
            {
                while (breakpoint)
                {
                    if (nextStep)
                    {
                        nextStep = false;
                        break;
                    }
                }
            }

            public void Interact(Context ctx, AstProvider.Base ast, Operation op)
            {
                Debug("Interact is not setup.");
                breakpoint = false;
            }
        }

        public class ProcessState
        {
            public bool exit = false;
            public bool pending = false;
            public Context last = null;
        }

        public class LoopState
        {
            public bool isBreak = false;
            public bool isContinue = false;
        }

        public class FunctionState
        {
            public CustomValue value = CustomNil.Void;
            public bool isReturn = false;
        }

        public class Options
        {
            public string target;
            public Context previous;
            public Type type;
            public State state;
            public bool isProtected;
            public bool injected;
            public Debugger debugger;
            public OutputHandler outputHandler;
            public CPS cps;
            public ProcessState processState;

            public Options(
                string? target,
                Context? previous,
                Type? type,
                State? state,
                bool? isProtected,
                bool? injected,
                Debugger? debugger,
                OutputHandler? outputHandler,
                CPS? cps,
                ProcessState? processState
            )
            {
                this.target = target ?? "unknown";
                this.previous = previous ?? null;
                this.type = type ?? Type.Api;
                this.state = state ?? State.Default;
                this.isProtected = isProtected ?? false;
                this.injected = injected ?? false;
                this.debugger = debugger ?? new Debugger();
                this.outputHandler = outputHandler ?? new DefaultOutputHandler();
                this.cps = cps ?? null;
                this.processState = processState ?? new ProcessState();
            }
        }

        public string target;
        public AstProvider.Base stackItem;
        public Debugger debugger;
        public OutputHandler outputHandler;
        public Context previous;
        public Type type;
        public State state;
        public Scope scope;
        public CPS cps;

        public ProcessState processState;
        public LoopState loopState;
        public FunctionState functionState;

        public bool isProtected;
        public bool injected;

        public Context api;
        public Context locals;
        public Context globals;

        public readonly static List<Type> lookupApiType = new() { Type.Api };
        public readonly static List<Type> lookupGlobalsType = new() { Type.Global };
        public readonly static List<Type> lookupLocalsType = new() { Type.Global, Type.Function };

        public Context(Options options)
        {
            target = options.target;
            stackItem = null;
            previous = options.previous;
            type = options.type;
            state = options.state;
            scope = new Scope(this);
            isProtected = options.isProtected;
            injected = options.injected;
            debugger = options.debugger;
            outputHandler = options.outputHandler;
            cps = options.cps;
            processState = options.processState;

            api = LookupApi();
            globals = LookupGlobals();
            locals = LookupLocals() ?? this;
        }

        public void Step(Operation entity)
        {
            if (!injected)
            {
                stackItem = entity.item;
                target = entity.target ?? target;
                SetLastActive(this);

                if (debugger.GetBreakpoint(this))
                {
                    debugger.Interact(this, entity.item, entity);
                    debugger.Resume();
                    return;
                }
            }
        }

        public Context SetLastActive(Context ctx)
        {
            if (!ctx.injected)
            {
                processState.last = ctx;
            }
            return this;
        }

        public Context GetLastActive()
        {
            return processState.last;
        }

        public bool IsExit()
        {
            return processState.exit;
        }

        public bool IsPending()
        {
            return processState.pending;
        }

        public Context SetPending(bool pending)
        {
            processState.pending = pending;
            return this;
        }

        public void Exit()
        {
            if (processState.pending)
            {
                processState.exit = true;
                return;
            }

            throw new InterpreterException("No running process was found.");
        }

        public Context LookupType(List<Type> allowedTypes)
        {
            if (allowedTypes.Contains(type))
            {
                return this;
            }

            Context current = previous;

            while (current != null)
            {
                if (allowedTypes.Contains(current.type))
                {
                    return current;
                }

                current = current.previous;
            }

            return null;
        }

        public Context LookupApi()
        {
            return LookupType(lookupApiType);
        }

        public Context LookupGlobals()
        {
            return LookupType(lookupGlobalsType);
        }

        public Context LookupLocals()
        {
            return LookupType(lookupLocalsType);
        }

        public Context Extend(Dictionary<string, CustomValue> map)
        {
            if (state == State.Temporary)
            {
                previous?.Extend(map);
            }
            else
            {
                scope.Extend(map);
            }

            return this;
        }

        public void Set(string path, CustomValue value)
        {
            Set(new Path<string>(path), value);
        }

        public void Set(Path<string> path, CustomValue value)
        {
            Path<string> traversalPath = new(path);
            string current = traversalPath.Next();

            if (current == "locals")
            {
                locals.Set(traversalPath, value);
            }
            else if (current == "globals")
            {
                globals.Set(traversalPath, value);
            }
            else if (state == State.Temporary)
            {
                previous?.Set(path, value);
            }
            else
            {
                locals.scope.Set(path, value);
            }
        }

        public CustomValue Get(string path)
        {
            return Get(new Path<string>(path));
        }

        public CustomValue Get(Path<string> path)
        {
            Path<string> traversalPath = new(path);
            string current = traversalPath.Next();

            if (current == "locals")
            {
                return locals.Get(traversalPath);
            }
            else if (current == "globals")
            {
                return globals.Get(traversalPath);
            }
            else if (state == State.Temporary)
            {
                return previous?.Get(path);
            }
            else
            {
                return locals.scope.Get(path);
            }
        }

        public Context Fork(Type type, State state)
        {
            return Fork(type, state, null, null);
        }

        public Context Fork(Type type, State state, string? target, bool? injected)
        {
            Options options = new(
                target ?? this.target,
                this,
                type,
                state,
                false,
                injected ?? this.injected,
                debugger,
                outputHandler,
                cps,
                processState
            );
            Context newContext = new(options);

            if (type != Type.Function)
            {
                if (type != Type.Loop)
                {
                    newContext.loopState = loopState;
                }

                newContext.functionState = functionState;
            }

            return newContext;
        }
    }
}
