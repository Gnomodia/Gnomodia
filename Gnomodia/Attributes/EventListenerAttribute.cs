using System;

namespace Gnomodia.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EventListenerAttribute : Attribute
    {
    }
}