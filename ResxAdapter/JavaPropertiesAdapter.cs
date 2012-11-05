using System;
using ResEx.Core;

namespace ResEx.StandardAdapters
{
    public class JavaPropertiesAdapter : IResourceBundleAdapter
    {
        #region Implementation of IResourceBundleAdapter

        public ResourceBundle Load(string fileName)
        {
            return null;
        }

        public void Save(string fileName, ResourceBundle resourceBundle)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}