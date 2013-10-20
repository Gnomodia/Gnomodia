using System;

namespace Gnomodia.Utility
{
    public static class EventHandlerExtensions
    {
        public static void TryRaise<T, T2, T3>(this EventHandler<EventArgs<T, T2, T3>> handler, object self, T arg1, T2 arg2, T3 arg3)
        {
            if (handler != null)
            {
                handler.Invoke(self, new EventArgs<T, T2, T3>(arg1, arg2, arg3));
            }
        }
        public static void TryRaise<T, T2>(this EventHandler<EventArgs<T, T2>> handler, object self, T arg1, T2 arg2)
        {
            if (handler != null)
            {
                handler.Invoke(self, new EventArgs<T, T2>(arg1, arg2));
            }
        }
        public static void TryRaise<T>(this EventHandler<EventArgs<T>> handler, object self, T args)
        {
            if (handler != null)
            {
                handler.Invoke(self, new EventArgs<T>(args));
            }
        }
        public static void TryRaise(this EventHandler handler, object self)
        {
            if (handler != null)
            {
                handler.Invoke(self, new EventArgs());
            }
        }
        public static void TryRaise<T>(this EventHandler<T> handler, object self, T args) where T : EventArgs
        {
            if (handler != null)
            {
                handler.Invoke(self, args);
            }
        }
    }
}