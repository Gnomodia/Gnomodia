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
using System.Text.RegularExpressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Gnomodia.Utility;

namespace Gnomodia
{
    /*

    public abstract class AnyEnumModification : IModification
    {
        private Type mEnumToChange;
        public Type EnumToChange
        {
            get { return mEnumToChange; }
        }
        public AnyEnumModification(Type enum_to_change)
        {
        }
    }
     
     */
    public abstract class EnumModification: IModification
    {
        public Type EnumToChange { get; private set; }
        public EnumModification(Type enum_to_change)
        {
            if (enum_to_change == null)
            {
                throw new ArgumentException("No enum type referenced.");
            }
            if (!enum_to_change.IsEnum)
            {
                throw new ArgumentException("Given type [" + enum_to_change.FullName + "] is not an enum.");
            }
            EnumToChange = enum_to_change;
        }
        Type IModification.TargetType
        {
            get
            {
                return EnumToChange;
            }
        }
    }
    public class EnumAddElement : EnumModification
    {
        public Object NewEnumValue { get; private set; }
        public String NewEnumName { get; private set; }
        public EnumAddElement(Type enum_to_change, string new_item_name, object value)
            : base(enum_to_change)
        {
            Init(new_item_name, value);
        }
        public EnumAddElement(Type enum_to_change, string new_item_name)
            : base(enum_to_change)
        {
            Init(new_item_name, null);
        }
        private void Init(string name, object value)
        {
            if ((name == null) || (name.Length == 0))
            {
                throw new ArgumentException("No name given for new element of enum [" + EnumToChange.FullName + "]");
            }
            if (!new Regex(@"[a-z][a-z0-9]+", RegexOptions.IgnoreCase).IsMatch(name))
            {
                throw new ArgumentException("Name [" + name + "] does not match /[a-z][a-z0-9]+/");
            }
            NewEnumValue = value;
            NewEnumName = name;
        }
    }
    public class EnumAddElement<T> : EnumAddElement where T : struct
    {
        public EnumAddElement(Type enum_to_change, string new_item_name, T value)
            : base(enum_to_change, new_item_name, value)
        {
            if (Enum.GetUnderlyingType(enum_to_change) != typeof(T))
            {
                throw new ArgumentException("Underlying type [" + Enum.GetUnderlyingType(enum_to_change).FullName + "] of enum [" + enum_to_change.FullName + "] does not match the specified type [" + typeof(T).FullName + "]");
            }
        }
    }
}
