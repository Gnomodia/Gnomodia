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

/*
 * We need this silly little trick because Settings change from one app domain to the other.
 * By having the Settings class implement the ISettings interface, we can do a special
 * trick in the Reference class that allows it to read setting through a MarshalByRef proxy
 * in the original app domain.
 */

namespace Gnomodia
{
    internal interface ISettings
    {
        string GnomoriaInstallationPath { get; set; }
    }
}

namespace Gnomodia.Properties
{
    partial class Settings : ISettings { }
}
