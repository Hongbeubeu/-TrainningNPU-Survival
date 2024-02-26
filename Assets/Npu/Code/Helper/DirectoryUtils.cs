using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Npu.Helper
{
    public static class DirectoryUtils
    {
        public static IEnumerable<string> EnumerateFiles(this string root, string searchPattern = "*")
        {
            if (!Directory.Exists(root)) throw new FileNotFoundException($"{root} is not a valid directory");

            foreach (var i in Directory.EnumerateFiles(root, searchPattern))
            {
                if (!i.EndsWith(".meta") && !Path.GetFileName(i).StartsWith(".")) yield return i;
            }

            foreach (var sub in Directory.EnumerateDirectories(root))
            {
                foreach (var i in EnumerateFiles(sub))
                {
                    yield return i;
                }
            }
        }
        
        public static IEnumerable<string> EnumerateDirectories(this string root, string searchPattern = "*")
        {
            if (!Directory.Exists(root)) throw new FileNotFoundException($"{root} is not a valid directory");

            foreach (var sub in Directory.EnumerateDirectories(root, searchPattern))
            {
                yield return sub;
                
                foreach (var i in EnumerateDirectories(sub))
                {
                    yield return i;
                }
            }
        }

        public static string TrimBeginningPathComponents(this string path, int parts)
        {
            return string.Join("/",
                path.Split(new []{ Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar})
                    .Skip(parts)
            );
        }
        

        public static IEnumerable<string> AncestorDirectories(this string path)
        {
            path = Path.GetDirectoryName(path);
            while (!string.IsNullOrEmpty(path))
            {
                yield return path;
                path = Path.GetDirectoryName(path);
            }
        }

        public static bool IsInsideOf(this string path, string other)
        {
            var info = new DirectoryInfo(path);
            var otherInfo = new DirectoryInfo(other);
            while (info != null)
            {
                if (otherInfo.FullName == info.FullName) return true;
                info = info.Parent;
            }
            return false;
        }
        
    }
}