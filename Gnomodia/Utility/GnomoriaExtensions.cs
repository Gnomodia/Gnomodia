using System;
using Game.GUI.Controls;

namespace Gnomodia.Utility
{
    public static class GnomoriaExtensions
    {
        public static bool FindControlRecursive<T>(this Control containingControl, Predicate<T> controlPredicate, out T foundControl) where T : Control
        {
            if (containingControl is T && controlPredicate((T)containingControl))
            {
                foundControl = (T) containingControl;
                return true;
            }

            foreach (var control in containingControl.ControlsList)
            {
                if (control.FindControlRecursive(controlPredicate, out foundControl))
                    return true;
            }

            foundControl = null;
            return false;
        }
    }
}