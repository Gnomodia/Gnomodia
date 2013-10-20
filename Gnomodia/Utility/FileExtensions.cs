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
using System.IO;
using System.Security.Cryptography;

namespace Gnomodia.Utility
{
    public static class FileExtensions
    {
        public static string GenerateMd5Hash(this FileInfo file)
        {
            if (file == null || !file.Exists)
                return null;
            var md5 = MD5.Create();
            using (var stream = new BufferedStream(file.OpenRead(), 1200000))
            {
                return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "");
            }
        }

        public static bool GetRelativePathTo(string path, string relativePathBase, out string relativePath)
        {
            path = Path.GetFullPath(path + "\\");
            relativePath = Uri.UnescapeDataString(
                new Uri(path)
                    .MakeRelativeUri(new Uri(relativePathBase))
                    .ToString()
                    .Replace('/', Path.DirectorySeparatorChar)
                );
            return !relativePath.StartsWith("..") && !relativePath.StartsWith(path);
            //!(relative_path.Contains('/') || relative_path.Contains('\\'));
        }
    }
}