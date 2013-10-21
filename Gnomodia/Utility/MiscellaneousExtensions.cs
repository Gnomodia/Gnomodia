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
using System.Reflection;
using System.Text;

namespace Gnomodia.Utility
{
    public static class MiscellaneousExtensions
    {
        public static string ToString(this Version version, string prefix = "")
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (!String.IsNullOrEmpty(prefix))
            {
                stringBuilder.Append(prefix);
                stringBuilder.Append(" ");
            }
            stringBuilder.AppendFormat("v{0}.{1}", version.Major, version.Minor);
            if (version.Build > 0 || version.Revision > 0)
            {
                stringBuilder.AppendFormat(".{0}", version.Build);
            }
            if (version.Revision > 0)
            {
                stringBuilder.AppendFormat(".{0}", version.Revision);
            }
            return stringBuilder.ToString();
        }

        public static string GetInformationalVersion(this Assembly assembly)
        {
            var versionAttribute = Attribute.GetCustomAttribute(assembly, typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute;
            return versionAttribute == null ? assembly.GetName().Version.ToString("") : versionAttribute.InformationalVersion;
        }
    }
}
