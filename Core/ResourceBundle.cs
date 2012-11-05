using System;
using System.Collections;
using System.Collections.Generic;

namespace ResEx.Core
{
    /// <summary>
    /// Represents a set of resource files with the same content but for different culture.
    /// For instance it may represent a collection of resx with the same base name but with different culture suffix (eg. en-US)
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "The fact that this implements ICollection is coinsidental. Maybe this will be changed.")]
    public class ResourceBundle : IEnumerable<ResourceSet>, ICollection<ResourceSet>
    {
        private readonly IDictionary<string, ResourceSet> contents = new Dictionary<string, ResourceSet>();

        public void CultureRemove(string culture)
        {
            this.contents[culture].Status = ResourceSetStatus.Deleted;
        }

        public bool ContainsCulture(string culture)
        {
            if (this.contents.ContainsKey(culture))
            {
                return this.contents[culture].Status != ResourceSetStatus.Deleted;
            }
            else
            {
                return false;
            }
        }

        public ResourceSet this[string culture]
        {
            get
            {
                return this.contents[culture];
            }
        }

        public ResourceSet Add(string culture)
        {
            var item = new ResourceSet(culture);
            item.Status = ResourceSetStatus.New;
            this.Add(item);
            return item;
        }

        #region Implementation of IEnumerable

        public IEnumerator<ResourceSet> GetEnumerator()
        {
            return this.contents.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<ResourceSet>

        public void Add(ResourceSet item)
        {
            this.contents.Add(item.Culture, item);
        }

        public void Clear()
        {
            this.contents.Clear();
        }

        public bool Contains(ResourceSet item)
        {
            return this.contents.Values.Contains(item);
        }

        public void CopyTo(ResourceSet[] array, int arrayIndex)
        {
            this.contents.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(ResourceSet item)
        {
            return this.contents.Remove(item.Culture);
        }

        public int Count
        {
            get
            {
                return this.contents.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}