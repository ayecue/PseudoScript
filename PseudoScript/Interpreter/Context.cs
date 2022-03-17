using PseudoScript.Interpreter.Operations;
using PseudoScript.Interpreter.Types;
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
            private readonly Context context;

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
                else if (path.Count == 1 && CustomMap.Intrinsics.Has(current))
                {
                    return CustomMap.Intrinsics.Get(current);
                }
                else if (context.previous != null)
                {
                    return context.previous.Get(path);
                }

                return Default.Void;
            }
        }

        public class Debugger
        {
            private bool breakpoint = false;
            private bool nextStep = false;
            private Context lastContext = null;

            public Context LastContext
            {
                get
                {
                    return lastContext;
                }
                internal set
                {
                    lastContext = value;
                }
            }

            public CustomNil Debug(string message)
            {
                Console.WriteLine(message);
                return Default.Void;
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
            private bool exit = false;
            private bool pending = false;
            private Context last = null;

            public bool IsExit
            {
                get
                {
                    return exit;
                }
                internal set
                {
                    exit = value;
                }
            }

            public bool IsPending
            {
                get
                {
                    return pending;
                }
                internal set
                {
                    pending = value;
                }
            }

            public Context Last
            {
                get
                {
                    return last;
                }
                internal set
                {
                    last = value;
                }
            }
        }

        public class LoopState
        {
            private bool isBreak = false;
            private bool isContinue = false;

            public bool IsBreak
            {
                get
                {
                    return isBreak;
                }
                internal set
                {
                    isBreak = value;
                }
            }

            public bool IsContinue
            {
                get
                {
                    return isContinue;
                }
                internal set
                {
                    isContinue = value;
                }
            }
        }

        public class FunctionState
        {
            private CustomValue value = Default.Void;
            private bool isReturn = false;

            public CustomValue Value
            {
                get
                {
                    return value;
                }
                internal set
                {
                    this.value = value;
                }
            }

            public bool IsReturn
            {
                get
                {
                    return isReturn;
                }
                internal set
                {
                    isReturn = value;
                }
            }
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
            public HandlerContainer handler;
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
                HandlerContainer? handler,
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
                this.handler = handler ?? new HandlerContainer();
                this.cps = cps ?? null;
                this.processState = processState ?? new ProcessState();
            }
        }

        public string target { get; private set; }
        public AstProvider.Base stackItem { get; private set; }
        public Debugger debugger { get; internal set; }
        public HandlerContainer handler { get; internal set; }
        public Context previous { get; private set; }

        public readonly Type type;
        public readonly State state;
        public readonly Scope scope;
        public readonly CPS cps;

        public readonly ProcessState processState;
        public LoopState loopState { get; internal set; }
        public FunctionState functionState { get; internal set; }

        public bool isProtected { get; private set; }
        public bool injected { get; private set; }

        private readonly Context api;
        private readonly Context locals;
        private readonly Context globals;

        private readonly static List<Type> lookupApiType = new() { Type.Api };
        private readonly static List<Type> lookupGlobalsType = new() { Type.Global };
        private readonly static List<Type> lookupLocalsType = new() { Type.Global, Type.Function };

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
            handler = options.handler;
            cps = options.cps;
            processState = options.processState;

            api = LookupApi();
            globals = LookupGlobals();
            locals = LookupLocals() ?? this;
        }

        internal void Step(Operation entity)
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

        internal Context SetLastActive(Context ctx)
        {
            if (!ctx.injected)
            {
                processState.Last = ctx;
            }
            return this;
        }

        public Context GetLastActive()
        {
            return processState.Last;
        }

        public bool IsExit()
        {
            return processState.IsExit;
        }

        public bool IsPending()
        {
            return processState.IsPending;
        }

        internal Context SetPending(bool pending)
        {
            processState.IsPending = pending;
            return this;
        }

        public void Exit()
        {
            if (processState.IsPending)
            {
                processState.IsExit = true;
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

        internal Context Extend(Dictionary<string, CustomValue> map)
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

        public Context Fork(Type type, State state, bool injected)
        {
            return Fork(type, state, null, true);
        }

        public Context Fork(Type type, State state, string target)
        {
            return Fork(type, state, target, null);
        }

        private Context Fork(Type type, State state, string? target, bool? injected)
        {
            Options options = new(
                target ?? this.target,
                this,
                type,
                state,
                false,
                injected ?? this.injected,
                debugger,
                handler,
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
