using System;
using System.Collections.Generic;
using System.IO;

namespace ResEx.StandardAdapters.Common
{
    public class FileSystemMockUp : IFileSystem
    {
        private readonly IEnumerable<string> files;

        public FileSystemMockUp(IEnumerable<string> files)
        {
            this.files = files;
        }

        public IEnumerable<string> GetFiles(string directoryName, string searchPattern, SearchOption searchOption)
        {
            return files;
        }

        public bool FileExists(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}