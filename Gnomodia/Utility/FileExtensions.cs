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