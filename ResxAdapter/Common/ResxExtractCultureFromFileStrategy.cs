using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using ResEx.Core;

namespace ResEx.StandardAdapters.Common
{
    public class ResxExtractCultureFromFileStrategy : IExtractCultureFromFileStrategy
    {
        /// <summary>
        /// Regular expression that extracts the culture string out of a resex file name.
        /// </summary>
        /// <remarks></remarks>
        private const string RegExGetCulture = "(?i:)(?<=\\.)\\D\\D(?:-\\D{2,4}?(?:-\\D\\D\\D\\D)?)?(?={extension})";
        private const string RegExReplaceCulture = "(?i:).(?<=\\.)\\D\\D(?:-\\D{2,4}?(?:-\\D\\D\\D\\D)?)?(?={extension})";

        public string GetCulture(string fileName)
        {
            Match cultureMatch = Regex.Match(fileName, GetRegularExpression(RegExGetCulture, fileName));
            if (cultureMatch == null || !cultureMatch.Success)
            {
                return ResourceSet.NeutralCulture;
            }

            return cultureMatch.Value;
        }

        public string ReplaceCulture(string fileName, string value)
        {
            return Regex.Replace(fileName, GetRegularExpression(RegExReplaceCulture, fileName), value);
        }

        private static string GetRegularExpression(string baseExpr, string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            return baseExpr.Replace("{extension}", fileInfo.Extension);
        }
    }
}