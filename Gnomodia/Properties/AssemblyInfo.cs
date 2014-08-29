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

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Gnomodia")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Gnomodia")]
[assembly: AssemblyProduct("Gnomodia")]
[assembly: AssemblyCopyright("Copyright © Gnomodia 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("02958cde-03cf-43f4-9ead-39d820bf1a1a")]

/*
 * Gnomodia uses semantic versioning! (http://semver.org/)
 * Do not change these numbers haphazardly! In short, this
 * is how it works:
 * 
 * The version format is X.Y.Z (Major.Minor.Patch).
 * Fixes not affecting the API increment the patch version,
 * backwards compatible API additions/changes increment the minor version,
 * and backwards incompatible API changes increment the major version.
 * 
 * Major version zero (0.y.z) is for initial development. Anything may change at any time.
 * The public API should not be considered stable.
 * 
 * Version 1.0.0 defines the public API. The way in which the version number is incremented
 * after this release is dependent on this public API and how it changes.
 * 
 * For complete information, please read http://semver.org/
 */
[assembly: AssemblyVersion(AssemblyResources.AssemblyBaseVersion)]
[assembly: AssemblyFileVersion(AssemblyResources.AssemblyBaseVersion)]
[assembly: AssemblyInformationalVersion(AssemblyResources.AssemblyBaseVersion + AssemblyResources.AssemblyPreReleaseVersion + "+" + AssemblyResources.GnomoriaTargetVersion)]

[assembly: InternalsVisibleTo("GnomodiaUI")]

internal static class AssemblyResources
{
    internal const string AssemblyBaseVersion = "0.1.0";
    internal const string AssemblyPreReleaseVersion = "-alpha";
    internal const string GnomoriaTargetVersion = "0.9.10";
}
