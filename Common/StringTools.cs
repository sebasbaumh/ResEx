using System;
using System.IO;

namespace ResEx.Common
{
    /// <summary>
    /// Provides some methods to handle strings
    /// </summary>
    /// <remarks></remarks>
    public static class StringTools
    {
        /// <summary>
        /// Returns the parent folder of the file that contains the given type
        /// </summary>
        public static string GetTypeFolder(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var fileInfo = new FileInfo(type.Assembly.ManifestModule.FullyQualifiedName);
            return fileInfo.DirectoryName;
        }

        /// <summary>
        /// Returns the number of times searchKey is contained in given text
        /// </summary>
        public static int CountInstances(string text, string searchKey, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(searchKey))
            {
                throw new ArgumentNullException("searchKey");
            }

            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            var counter = 0;
            int location = 0;
            while (true)
            {
                location = text.IndexOf(searchKey, location, comparisonType);
                if (location == -1) break;
                counter++;
                location += searchKey.Length;
            }

            return counter;
        }

        /// <summary>
        /// Returns true if given value is null or empty string
        /// </summary>
        public static bool ValueNullOrEmpty(object value)
        {
            if (value == null)
            {
                return true;
            }
            else
            {
                var valueAsString = value as string;

                if (valueAsString != null)
                {
                    return string.IsNullOrEmpty(valueAsString);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Compacts the given text to fit in the given length by removing characters
        /// from the middle. Uses native windows algorithm that trims long paths for display purposes.
        /// </summary>
        public static string CompactText(string path, int maxLength)
        {
            string fileNameStipped = new string('\0', 2000);
            var result = NativeMethods.PathCompactPathEx(fileNameStipped, path, maxLength, 1);
            if (result != 1)
            {
                throw new InvalidOperationException("Native method returned " + result);
            }

            // trim null characters from buffer
            var indexOfNull = fileNameStipped.IndexOf('\0');
            if (indexOfNull != -1)
            {
                fileNameStipped = fileNameStipped.Substring(0, indexOfNull);
            }

            return fileNameStipped;
        }
    }
}