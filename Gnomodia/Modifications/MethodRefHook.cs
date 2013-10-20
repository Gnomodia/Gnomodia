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
    public class MethodRefHook : UnmutableMethodModification
    {
        public MethodRefHook(MethodBase intercepted, MethodInfo custom_method, MethodHookFlags hook_flags = MethodHookFlags.None)
            : base(intercepted, custom_method, MethodHookType.RunBefore, hook_flags)
        {
            if (hook_flags == MethodHookFlags.CanSkipOriginal)
            {
                throw new NotImplementedException("RefHook does not yet support Skipping. Want that feature? Leave me a note, so i may actually implement it :)");
            }
        }
        public override IEnumerable<CustomParameterInfo> GetRequiredParameterLayout()
        {
            if (!InterceptedMethod.IsStatic)
            {
                yield return InterceptedMethod.DeclaringType;
            }
            foreach (var el in InterceptedMethod.GetParameters())
            {
                if (el.ParameterType.IsByRef)
                {
                    yield return el;
                }
                else
                {
                    yield return el.ParameterType.MakeByRefType();
                }
            }
        }
    }
}
