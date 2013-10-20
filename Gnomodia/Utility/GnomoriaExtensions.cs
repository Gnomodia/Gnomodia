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