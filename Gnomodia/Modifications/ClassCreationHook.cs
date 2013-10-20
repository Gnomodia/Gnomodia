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
    /// <summary>
    /// Allows you to call your own method instead of a specific object constructor in selected functions
    /// </summary>
    [Obsolete("Do not use this shit! It is very early wip and you will only break sth...")]
    public class ClassCreationHook: IModification
    {
        public Type ClassToInterceptCreation { get; private set; }
        public MethodBase InterceptCreationInMethod { get; private set; }
        public MethodBase CustomCreationMethod { get; private set; }

        Type IModification.TargetType { get { return InterceptCreationInMethod == null ? ClassToInterceptCreation : InterceptCreationInMethod.DeclaringType; } }

        // Todo: make it more "generic", might by specifiying a base class and letting all others pass to it. Also an "ANY" instead of doChangeIn would be nice.
        public ClassCreationHook(Type classToIncerceptCreation, MethodBase doChangeInMethod, MethodBase customCalledCreatingFunction)
        {
            ClassToInterceptCreation = classToIncerceptCreation;
            InterceptCreationInMethod = doChangeInMethod;
            CustomCreationMethod = customCalledCreatingFunction;
            // Todo: Find a new solution for arguments
            // Todo: NO CHECKS HERE, YET!
        }
       
    }
}