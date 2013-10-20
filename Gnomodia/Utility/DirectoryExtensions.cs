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