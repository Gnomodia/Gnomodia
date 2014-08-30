using System;

namespace Gnomodia.Events
{
    /// <summary>
    /// This is the event where you can be certain all other mods have initialized,
    /// so that you may safely access their provided features.
    /// </summary>
    public class PostGameInitializeEventArgs : EventArgs
    {
    }
}