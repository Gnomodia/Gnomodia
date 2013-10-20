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

using System.IO;

namespace Gnomodia.Utility
{
    internal static class DirectoryExtensions
    {
        public static FileInfo GetFile(this DirectoryInfo self, string fileName)
        {
            return new FileInfo(Path.Combine(self.FullName, fileName));
        }
        public static DirectoryInfo GetDirectory(this DirectoryInfo self, string fileName)
        {
            return new DirectoryInfo(Path.Combine(self.FullName, fileName));
        }
    }
}