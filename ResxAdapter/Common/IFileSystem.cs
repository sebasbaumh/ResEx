using System.Collections.Generic;
using System.IO;

namespace ResEx.StandardAdapters.Common
{
    public interface IFileSystem
    {
        IEnumerable<string> GetFiles(string directoryName, string searchPattern, SearchOption searchOption);

        bool FileExists(string fileName);
    }
}