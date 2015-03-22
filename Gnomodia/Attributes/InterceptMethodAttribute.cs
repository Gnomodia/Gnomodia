/*
 *  Gnomodia
 *
 *  Copyright © 2014 Alexander Krivács Schrøder (https://alexanderschroeder.net/)
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
using System.Reflection;

namespace Gnomodia.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class InterceptMethodAttribute : Attribute
    {
        public MethodBase InterceptedMethod { get; set; }
        public MethodHookType HookType { get; set; }
        public MethodHookFlags HookFlags { get; set; }

        public InterceptMethodAttribute(Type interceptedType, string interceptedMethodName, BindingFlags bindingFlags)
            : this(interceptedType.GetMethod(interceptedMethodName, bindingFlags))
        {
        }

        public InterceptMethodAttribute(Type interceptedType, string interceptedMethodName)
            : this(interceptedType.GetMethod(interceptedMethodName))
        {
        }

        public InterceptMethodAttribute(MethodBase interceptedMethod)
        {
            InterceptedMethod = interceptedMethod;
            HookType = MethodHookType.RunAfter;
            HookFlags = MethodHookFlags.None;
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class InterceptConstructorAttribute : InterceptMethodAttribute
    {
        public InterceptConstructorAttribute(Type interceptedType)
            : this(interceptedType, new Type[] { })
        {
        }
        public InterceptConstructorAttribute(Type interceptedType, Type[] types)
            : base(interceptedType.GetConstructor(types))
        {
        }
    }

}
