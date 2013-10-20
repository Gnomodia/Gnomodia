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
using Gnomodia;

namespace Gnomodia
{
    //http://web.archive.org/web/20120124210217/http://evain.net/blog/articles/2010/05/05/parameterof-propertyof-methodof

    public static class Method
    {
        public static MethodInfo Of(Delegate d)
        {
            return d.Method;
        }

        public static MethodInfo Of<TRet>(Func<TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, TRet>(Func<T1, TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, T2, TRet>(Func<T1, T2, TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, T2, T3, T4, T5, T6, TRet>(Func<T1, T2, T3, T4, T5, T6, TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, T2, T3, T4, T5, T6, T7, TRet>(Func<T1, T2, T3, T4, T5, T6, T7, TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, T2, T3, T4, T5, T6, T7, T8, TRet>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet> value)
        {
            return value.Method;
        }
        
        public static MethodInfo Of(Action value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1>(Action<T1> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, T2>(Action<T1, T2> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, T2, T3>(Action<T1, T2, T3> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, T2, T3, T4>(Action<T1, T2, T3, T4> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> value)
        {
            return value.Method;
        }
        public static MethodInfo Of<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> value)
        {
            return value.Method;
        }

        public static MethodInfo Func<TRet>(Func<TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Func<T1, TRet>(Func<T1, TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Func<T1, T2, TRet>(Func<T1, T2, TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Func<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Func<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Func<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Func<T1, T2, T3, T4, T5, T6, TRet>(Func<T1, T2, T3, T4, T5, T6, TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Func<T1, T2, T3, T4, T5, T6, T7, TRet>(Func<T1, T2, T3, T4, T5, T6, T7, TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Func<T1, T2, T3, T4, T5, T6, T7, T8, TRet>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet> value)
        {
            return value.Method;
        }
        public static MethodInfo Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet> value)
        {
            return value.Method;
        }

        public static MethodInfo Action(Action value)
        {
            return value.Method;
        }
        public static MethodInfo Action<T1>(Action<T1> value)
        {
            return value.Method;
        }
        public static MethodInfo Action<T1, T2>(Action<T1, T2> value)
        {
            return value.Method;
        }
        public static MethodInfo Action<T1, T2, T3>(Action<T1, T2, T3> value)
        {
            return value.Method;
        }
        public static MethodInfo Action<T1, T2, T3, T4>(Action<T1, T2, T3, T4> value)
        {
            return value.Method;
        }
        public static MethodInfo Action<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> value)
        {
            return value.Method;
        }
        public static MethodInfo Action<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> value)
        {
            return value.Method;
        }
        public static MethodInfo Action<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> value)
        {
            return value.Method;
        }
        public static MethodInfo Action<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> value)
        {
            return value.Method;
        }
        public static MethodInfo Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> value)
        {
            return value.Method;
        }
        public static MethodInfo Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> value)
        {
            return value.Method;
        }

        public static T CreateDummy<T>()
        {
            return (T)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
            //return Activator.CreateInstance<T>();
        }
    }
}
