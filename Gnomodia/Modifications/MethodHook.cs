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
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Gnomodia.Utility;

namespace Gnomodia
{
    class MethodHook : UnmutableMethodModification
    {
        public MethodHook(MethodBase intercepted, MethodInfo custom_method, MethodHookType hook_type = MethodHookType.RunAfter, MethodHookFlags hook_flags = MethodHookFlags.None)
            :base(intercepted, custom_method, hook_type, hook_flags)
        {
        }
    }
    class BeforeAndAfterMethodHook : ModificationCollection
    {
        public override Type TargetType { get { return ((IModification)OnBeforeHook).TargetType; } }
        public MethodHook OnBeforeHook { get; protected set; }
        public MethodHook OnAfterHook { get; protected set; }
        public override IEnumerator<IModification> GetModifications()
        {
            yield return OnBeforeHook;
            yield return OnAfterHook;
        }
        public BeforeAndAfterMethodHook(MethodBase intercepted, MethodInfo custom_before_method, MethodInfo custom_after_method)
        {
            OnBeforeHook = new MethodHook(intercepted, custom_before_method, MethodHookType.RunBefore);
            OnAfterHook = new MethodHook(intercepted, custom_after_method, MethodHookType.RunAfter);
        }
    }
}