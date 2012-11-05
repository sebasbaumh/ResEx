using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ResEx.Common;

namespace ResEx.Core
{
    /// <summary>
    /// Represents a resource file of a specific culture.
    /// For instance it may represent one resx file.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "The fact that this inherics a dictionary is coinsidental. Maybe this will be changed.")]
    public class ResourceSet : IndexedDictionary<string, ResourceItem>
    {
        public const string NeutralCulture = "neutral";

        #region Events

        public event EventHandler<ResourceItemChangedEventArgs> ResourceItemChanged;

        private void InvokeResourceItemChanged(ResourceItem item)
        {
            EventHandler<ResourceItemChangedEventArgs> handler = this.ResourceItemChanged;
            if (handler != null)
            {
                handler(this, new ResourceItemChangedEventArgs { Item = item });
            }
        }

        #endregion

        #region Override standard add/remove methods in order to subscribe/unsubscribe to PropertyChanged event

        public override void Add(string key, ResourceItem value)
        {
            base.Add(key, value);
            value.PropertyChanged += this.ResourceItem_PropertyChanged;
        }

        public override bool Remove(string key)
        {
            base[key].PropertyChanged -= this.ResourceItem_PropertyChanged;
            return base.Remove(key);
        }

        /// <summary>
        /// When PropertyChanged event of an item of the collection is raised
        /// then propagate this event using <see cref="ResourceItemChanged"/> event of this
        /// class.
        /// </summary>
        /// <remarks>
        /// This event can be handle from the UI to update itself when items in the collection change
        /// from an externa source (eg. a plug in)
        /// </remarks>
        private void ResourceItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.InvokeResourceItemChanged((ResourceItem)sender);
        }

        #endregion

        public string Culture { get; internal set; }

        public ResourceSetStatus Status { get; set; }

        private List<Exclusion> _exclusions;

        public List<Exclusion> Exclusions
        {
            get
            {
                if (_exclusions == null)
                {
                    // put some global exlusions
                    this._exclusions = new List<Exclusion>
                                      {
                                          new Exclusion
                                              {
                                                  Pattern = @"\[.*:.*]",
                                                  Sample = "[Text:Text]"
                                              }
                                      };

                    // add to exclusions, item defined by the developer as resource metadata
                    this._exclusions.AddRange(this.GetExclusions());
                }

                return _exclusions;
            }
        }

        public ResourceSet(string culture)
        {
            this.Culture = culture;
        }

        /// <summary>
        /// Returns a list of items that should be excluded from translation
        /// </summary>
        private Exclusion[] GetExclusions()
        {
            // get items from neutral or first resource set (base for translation)
            // that have @Exclusion as comment
            var items = from p in this.Values
                        where p.Locked && p.LockedReason == LockedReason.ResexMetadata && p.Comment == "@Exclusion"
                        select (string)p.Value;

            var exlusions = new List<Exclusion>();
            foreach (var item in items)
            {
                // first part of value is the pattern and second part (optional) is a sample that matches the pattern
                var itemSplit = item.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                exlusions.Add(new Exclusion
                {
                    Pattern = itemSplit[0],
                    Sample = itemSplit.Length > 1 ? itemSplit[1] : string.Empty
                });
            }

            return exlusions.ToArray();
        }

        /// <summary>
        /// Returns item with given key exists and has a value
        /// </summary>
        /// <returns></returns>
        public bool HasValue(string key)
        {
            return this.ContainsKey(key) && !this[key].ValueEmpty;
        }

        /// <summary>
        /// Returns the string item with the given key. If item does not exist it will be created.
        /// </summary>
        /// <exception cref="InvalidOperationException">If item with given key exists but is not of type <see cref="ResourceStringItem"/></exception>
        public ResourceStringItem GetStringItem(string key)
        {
            ResourceStringItem item;
            if (this.ContainsKey(key))
            {
                item = this[key] as ResourceStringItem;
                if (item == null) throw new InvalidOperationException("Item with given key is not a string item");
            }
            else
            {
                item = new ResourceStringItem();
                item.Name = key;
                this.Add(key, item);
            }

            return item;
        }

        #region Statistics Methods

        /// <summary>
        /// Returns the number of translated items of the given base resource set in the current one
        /// </summary>
        public int CountTranslatedItems(ResourceSet baseResourceSet)
        {
            int translated = 0;

            foreach (var item in baseResourceSet)
            {
                if (item.Value is ResourceStringItem && !item.Value.Locked && this.ContainsKey(item.Key))
                {
                    ResourceItem localItem = this[item.Key];
                    if (localItem != null && !localItem.Locked && localItem.Value != null && !string.IsNullOrEmpty(localItem.Value.ToString()))
                    {
                        translated++;
                    }
                }
            }

            return translated;
        }

        public int CountStringItems(bool includeEmpty)
        {
            // count only the items that do have a value
            var items = from p in this.Values
                        where p is ResourceStringItem && (includeEmpty || !StringTools.ValueNullOrEmpty(p.Value))
                        select p;
            return items.Count();
        }

        public int CountLocked()
        {
            return this.Values.Where(p => p.Locked).Count();
        }

        public int CountWords()
        {
            int count = 0;
            foreach (ResourceItem item in Values)
            {
                // count words of non locked, non empty, string items
                var stringItem = item as ResourceStringItem;
                if (stringItem != null && stringItem.Value != null && !stringItem.Locked)
                {
                    count += stringItem.CountWords();
                }
            }

            return count;
        }

        public int CountMarkedForReviewing()
        {
            return this.Values.Where(p => p.ReviewPending).Count();
        }

        #endregion
    }
}