using System.Collections.Generic;
using System.IO;

namespace ResEx.StandardAdapters.Common
{
    public class FileSystem : IFileSystem
    {
        public IEnumerable<string> GetFiles(string directoryName, string searchPattern, SearchOption searchOption)
        {
            return Directory.GetFiles(directoryName, searchPattern, searchOption);
        }

        public bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }
    }
}