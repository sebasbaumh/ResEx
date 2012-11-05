using System.Linq;

namespace ResEx.Core
{
    public static class ResourceBundleExtensions
    {
        /// <summary>
        /// Returns the neutral resource set of the given bundle or the first one, if there is no neutral.
        /// </summary>
        public static ResourceSet NeutralOrFirst(this ResourceBundle resourceBundle)
        {
            if (resourceBundle == null) return null;

            var returnValue = resourceBundle.FirstOrDefault(p => p.Culture == ResourceSet.NeutralCulture);

            if (returnValue == null) returnValue = resourceBundle.FirstOrDefault();

            return returnValue;
        }
    }
}
