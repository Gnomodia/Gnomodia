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
using System.Reflection;

namespace Gnomodia
{
    interface IModification
    {
        Type TargetType { get; }
    }
    interface IModificationCollection : IModification, IEnumerable<IModification>
    {
        IEnumerator<IModification> GetModifications();
    }
    abstract class ModificationCollection : IModificationCollection
    {
        public abstract IEnumerator<IModification> GetModifications();
        public abstract Type TargetType { get; }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetModifications();
        }
        IEnumerator<IModification> IEnumerable<IModification>.GetEnumerator()
        {
            return GetModifications();
        }
    }

    interface IMethodModification : IModification
    {
    }

    public enum MethodHookType { RunBefore, RunAfter, Replace }
    [Flags]
    public enum MethodHookFlags
    {
        None = 0x0,
        CanSkipOriginal = 0x1
    }

    public class CustomParameterInfo
    {
        public Type ParameterType { get; private set; }
        public bool IsOut { get; private set; }
        public CustomParameterInfo(Type type, bool isOut = false)
        {
            ParameterType = (isOut && !type.IsByRef) ? type.MakeByRefType() : type;
            IsOut = isOut;
        }

        public bool IsSimpleType(Type t)
        {
            return !IsOut && (ParameterType == t);
        }

        public static implicit operator CustomParameterInfo(Type t)
        {
            return new CustomParameterInfo(t);
        }
        public static implicit operator CustomParameterInfo(ParameterInfo pi)
        {
            return new CustomParameterInfo(pi.ParameterType, pi.IsOut);
        }

        public static bool IsSimilar(CustomParameterInfo a, CustomParameterInfo b)
        {
            // TODO: generics are missing
            if (a == null || b == null)
                return false;
            if (a == b)
                return true;
            if (a.IsOut != b.IsOut)
                return false;
            if (a.ParameterType.IsByRef)
            {
                if (!b.ParameterType.IsByRef)
                    return false;
                return a.ParameterType.GetElementType() == b.ParameterType.GetElementType();
            }
            return a.ParameterType == b.ParameterType;
        }
        public bool IsSimilar(CustomParameterInfo trg)
        {
            return IsSimilar(this, trg);
        }
    }

    public abstract class UnmutableMethodModification : IMethodModification
    {
        public static bool IsCompilerGenerated(MethodInfo method)
        {
            return method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false).Length > 0;
        }
        public static void VerifyNestedPublicMethod(MethodInfo method)
        {
            var curType = method.DeclaringType;
            while (curType != null)
            {
                if (!curType.IsPublic && !(curType.IsNested && curType.IsNestedPublic))
                {
                    throw new InvalidOperationException(string.Format("One of the Types ({0}) declaring the custom method ({1}) is not public, making this method inaccessible!", curType.FullName, method.Name));
                }
                curType = curType.DeclaringType;
            }
        }
        private static string MethodReferenceToString(MethodBase method)
        {
            var declaringType = method.DeclaringType;
            if (declaringType != null)
            {
                return string.Format("{0}; Type {1}", method.Name, declaringType.FullName);
            }
            else
            {
                return method.Name;
            }
        }

        public MethodHookType HookType { get; private set; }
        public MethodBase InterceptedMethod { get; private set; }
        public MethodInfo CustomMethod{get; private set;}
        public MethodHookFlags HookFlags{get; private set;}

        Type IModification.TargetType
        {
            get
            {
                return InterceptedMethod.DeclaringType;
            }
        }

        protected virtual void Validate_1_NoInformationMissing()
        {
            if ((InterceptedMethod == null) || (CustomMethod == null))
            {
                if (InterceptedMethod == CustomMethod)
                {
                    throw new ArgumentException("Custom and intercepted methods are null. Can't create a hook without them!");
                }
                if (InterceptedMethod == null)
                {
                    throw new ArgumentException("Intercepted method is null. Can't create a hook without! Custom method is [" + MethodReferenceToString(CustomMethod) + "]");
                }
                throw new ArgumentException("Custom method is null. Can't create a hook without! Intercepted method is [" + MethodReferenceToString(InterceptedMethod) + "]");
            }
        }
        protected virtual void Validate_2_Accessibility()
        {
            if (!CustomMethod.IsPublic)
                throw new ArgumentException("Custom method has to be public! " + CustomMethod.Name + " is not.");
            if (IsCompilerGenerated(CustomMethod))
                throw new ArgumentException("Custom method can not be compiler generated");

            VerifyNestedPublicMethod(CustomMethod);
        }
        protected virtual void Validate_3_SpecialCases()
        {
            if (InterceptedMethod.IsConstructor && (HookType != MethodHookType.RunAfter))
                throw new ArgumentException("Intercepted Method appears to be a constructor for a type (" + InterceptedMethod.DeclaringType.FullName + "). Only MethodHookType.RunAfter is currently supported for constructors.");
            if (HookFlags.HasFlag(MethodHookFlags.CanSkipOriginal))
            {
                if (HookType != MethodHookType.RunBefore)
                {
                    throw new ArgumentException("MethodHookFlags.CanSkipOriginal requires MethodHookType.RunBefore!");
                }
            }
        }
        protected virtual void Validate_4_ParameterLayout()
        {
            var requredParameterLayout = GetRequiredParameterLayout().ToList();
            var foundParameterLayout = CustomMethod.GetParameters();
            var valid = requredParameterLayout.Count == foundParameterLayout.Length;
            for (var i = 0; (i < foundParameterLayout.Length) && valid; i++)
            {
                if (requredParameterLayout[i].ParameterType.IsByRef)
                {
                    if (foundParameterLayout[i].ParameterType.GetElementType() != requredParameterLayout[i].ParameterType.GetElementType())
                    {
                        valid = false;
                    }
                    if ((HookType != MethodHookType.RunAfter) && (foundParameterLayout[i].IsOut != requredParameterLayout[i].IsOut))
                    {
                        valid = false;
                    }
                }
                else if (foundParameterLayout[i].ParameterType != requredParameterLayout[i].ParameterType)
                {
                    if (!foundParameterLayout[i].ParameterType.IsValueType && !requredParameterLayout[i].ParameterType.IsValueType && requredParameterLayout[i].ParameterType.IsSubclassOf(foundParameterLayout[i].ParameterType))
                    {
                        //should be simple ptr stuff & fine to "convert". We can allow this
                    }
                    else
                    {
                        valid = false;
                    }
                }
            }
            if (!valid)
            {
                Func<IEnumerable<CustomParameterInfo>, String> toString = f => f.Select(t => t == null ? "NULL" : ((t.IsOut ? "out " : (t.ParameterType.IsByRef ? "ref " : "")) + (t.ParameterType.IsByRef ? t.ParameterType.GetElementType() : t.ParameterType).FullName)).Aggregate((f1, f2) => f1 + ", " + f2);
                throw new ArgumentException("Invalid parameter Layout for method [" + MethodReferenceToString(CustomMethod) + "]! Expected [" + toString(requredParameterLayout) + "], got [" + toString(foundParameterLayout.Select(el => (CustomParameterInfo)el)) + "].");
            }
        }
        private static void Validate_5_ReturnType_WrongReturnType(Type got, Type excpected, MethodBase func)
        {
            throw new ArgumentException("Invalid return type! Got [" + got.FullName + "] while expecting [" + excpected.FullName + "] at func [" + MethodReferenceToString(func) + "]");
        }
        protected virtual void Validate_5_ReturnType()
        {

            //Return value checking
            if (HookFlags.HasFlag(MethodHookFlags.CanSkipOriginal))
            {
                if (CustomMethod.ReturnType != typeof(bool))
                {
                    throw new ArgumentException("Methods that can skip the original method have to return a bool, indicating the skipping state");
                }
            }
            else if ((HasResultAsFirstParameter || (HookType == MethodHookType.Replace)) && (InterceptedMethod is MethodInfo))
            {
                var expectedType = (InterceptedMethod as MethodInfo).ReturnType;
                if (expectedType != CustomMethod.ReturnType)
                {
                    Validate_5_ReturnType_WrongReturnType(CustomMethod.ReturnType, expectedType, CustomMethod);
                }
            }
            else if (CustomMethod.ReturnType != typeof(void))
            {
                Validate_5_ReturnType_WrongReturnType(CustomMethod.ReturnType, typeof(void), CustomMethod);
            }
        }

        protected UnmutableMethodModification(MethodBase intercepted, MethodInfo customMethod, MethodHookType hookType = MethodHookType.RunAfter, MethodHookFlags hookFlags = MethodHookFlags.None)
        {
            HookType = hookType;
            InterceptedMethod = intercepted;
            CustomMethod = customMethod;
            HookFlags = hookFlags;

            Validate();
        }

        private void Validate()
        {
            Validate_1_NoInformationMissing();
            Validate_2_Accessibility();
            Validate_3_SpecialCases();
            Validate_4_ParameterLayout();
            Validate_5_ReturnType();
        }

        public virtual bool HasResultAsFirstParameter
        {
            get
            {
                var intmethAsMethInfo = InterceptedMethod as MethodInfo;
                return (intmethAsMethInfo != null) && (HookType == MethodHookType.RunAfter) && (intmethAsMethInfo.ReturnType != typeof(void));
            }
        }
        public virtual bool RequiresLocalToCacheOutResult
        {
            get
            {
                return HookFlags.HasFlag(MethodHookFlags.CanSkipOriginal) && (InterceptedMethod is MethodInfo) && ((InterceptedMethod as MethodInfo).ReturnType != typeof(void));
            }
        }
        public virtual IEnumerable<CustomParameterInfo> GetRequiredParameterLayout()
        {
            if (HasResultAsFirstParameter)
            {
                yield return (InterceptedMethod as MethodInfo).ReturnType;
            }
            if (!InterceptedMethod.IsStatic)
            {
                yield return InterceptedMethod.DeclaringType;
            }
            foreach (var param in InterceptedMethod.GetParameters())
            {
                yield return param;
            }
            if (RequiresLocalToCacheOutResult)
            {
                yield return new CustomParameterInfo((InterceptedMethod as MethodInfo).ReturnType, true);
            }
        }
    }


}
