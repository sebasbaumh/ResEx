namespace ResEx.Core
{
    /// <summary>
    /// When implemented loads resource information from files into <see cref="ResourceBundle"/> object.
    /// Acts as an adapter between specific resource file formats (such as .resx) and this application.
    /// </summary>
    public interface IResourceBundleAdapter
    {
        ResourceBundle Load(string fileName);

        void Save(string fileName, ResourceBundle resourceBundle);
    }
}