namespace ResEx.StandardAdapters.Common
{
    public interface IExtractCultureFromFileStrategy
    {
        /// <summary>
        /// Returns the culture string from the given file name. Returns null if file name is a neutral file and does not
        /// contain culture in file name.
        /// </summary>
        string GetCulture(string fileName);
    }
}