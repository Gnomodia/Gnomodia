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

namespace Gnomodia
{
    public abstract class ClassModification : IModification
    {
        public Type ClassToChange { get; private set; }

        protected ClassModification(Type classToChange)
        {
            if (classToChange == null)
            {
                throw new ArgumentException("No class type referenced.");
            }
            if (!classToChange.IsClass)
            {
                throw new ArgumentException("Given type " + classToChange.FullName + " is not a class.");
            }
            ClassToChange = classToChange;
        }

        Type IModification.TargetType
        {
            get
            {
                return ClassToChange;
            }
        }
    }

    public class ClassChangeBase : ClassModification
    {
        public Type NewBaseClass { get; private set; }
        public ClassChangeBase(Type classToChange, Type newBaseClass)
            : base(classToChange)
        {
            if (newBaseClass == null)
            {
                throw new ArgumentException("No new base type referenced.");
            }
            if (!newBaseClass.IsClass)
            {
                throw new ArgumentException("Provided base type " + newBaseClass.FullName + " is not a class.");
            }
            if (!classToChange.BaseType.IsAssignableFrom(newBaseClass))
            {
                throw new ArgumentException("The targets original base type " + classToChange.BaseType.FullName + " is not assignable from the given new base type " + newBaseClass.FullName + ".");
            }
            NewBaseClass = newBaseClass;
            // Todo: make sure the new class has all constructors. It should work without for now, but forcing it would / should result in more awareness
        }
    }
}
