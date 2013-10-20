using System;

namespace Gnomodia.Utility
{
    public class EventArgs<T> : EventArgs
    {
        public T Argument;
        public EventArgs(T arg)
        {
            Argument = arg;
        }
    }

    public class EventArgs<T, T2> : EventArgs<T>
    {
        public T2 Argument2;
        public EventArgs(T arg, T2 arg2)
            : base(arg)
        {
            Argument2 = arg2;
        }
    }
    
    public class EventArgs<T, T2, T3> : EventArgs<T, T2>
    {
        public T3 Argument3;
        public EventArgs(T arg, T2 arg2, T3 arg3)
            : base(arg, arg2)
        {
            Argument3 = arg3;
        }
    }
}