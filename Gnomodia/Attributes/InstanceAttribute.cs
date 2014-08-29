using System;

namespace Gnomodia.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class InstanceAttribute : Attribute
    {
    }
}