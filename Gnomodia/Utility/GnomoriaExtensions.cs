/*
 *  Gnomodia
 *
 *  Copyright © 2013 Faark (http://faark.de/)
 *  Copyright © 2013 Alexander Krivács Schrøder (https://alexanderschroeder.net/)
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU Lesser General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU Lesser General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Game.GUI.Controls;

namespace Gnomodia.Utility
{
    public static class GnomoriaExtensions
    {
        public static bool FindControlRecursive<T>(this Control containingControl, out T foundControl, Predicate<T> controlPredicate = null) where T : Control
        {
            if (containingControl is T && (controlPredicate == null || controlPredicate((T)containingControl)))
            {
                foundControl = (T)containingControl;
                return true;
            }

            foreach (var control in containingControl.ControlsList)
            {
                if (control.FindControlRecursive(out foundControl, controlPredicate))
                    return true;
            }

            foundControl = null;
            return false;
        }

        private static void FindControlsRecursive<T>(Control containingControl, Predicate<T> controlPredicate, ref List<T> foundControls) where T : Control
        {
            if (containingControl is T && (controlPredicate == null || controlPredicate((T)containingControl)))
                foundControls.Add((T)containingControl);

            foreach (var control in containingControl.ControlsList)
            {
                FindControlsRecursive(control, controlPredicate, ref foundControls);
            }
        }

        public static bool FindControlsRecursive<T>(this Control containingControl, out T[] foundControls, Predicate<T> controlPredicate = null) where T : Control
        {
            List<T> controls = new List<T>();
            FindControlsRecursive(containingControl, controlPredicate, ref controls);
            foundControls= controls.ToArray();
            return controls.Any();
        }

        public static void CalculateWidth(this Label label)
        {
            label.Width = (int)label.Skin.Layers[0].Text.Font.Resource.MeasureString(label.Text).X + 2;
        }
    }
}