using System;
using System.Collections;
using System.Collections.Generic;

namespace ResEx.Common
{
    [System.ComponentModel.DefaultProperty("Item")]
    public class RecentCollection : IEnumerable<RecentItem>
    {
        private readonly object syncObject = new object();

        private List<RecentItem> list = new List<RecentItem>();

        private int maxSize = 10;

        public event EventHandler<RecentItemActionEventArgs> ItemPushed;

        public event EventHandler<RecentItemActionEventArgs> ItemRemoved;

        /// <summary>
        /// Gets or sets a value that determines the maximum size of this instance
        /// </summary>
        public int MaxSize
        {
            get
            {
                return this.maxSize;
            }

            set
            {
                lock (this.syncObject)
                {
                    if (value < 3)
                    {
                        throw new ArgumentOutOfRangeException("value", "Length must be a least 3");
                    }

                    this.maxSize = value;
                    if (this.list.Count > value)
                    {
                        for (int i = this.list.Count; i >= value + 1; i += -1)
                        {
                            RecentItem item = this.list[i - 1];
                            if (this.ItemRemoved != null)
                            {
                                this.ItemRemoved(this, new RecentItemActionEventArgs(item));
                            }

                            this.list.Remove(item);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the number of elements actually contained.
        /// </summary>
        public int Count
        {
            get
            {
                if (this.list == null)
                {
                    return 0;
                }
                else
                {
                    return this.list.Count;
                }
            }
        }

        /// <summary>
        /// Gets the element at the specified index
        /// </summary>
        /// <returns>The element at the specified index</returns>
        public RecentItem this[int index]
        {
            get { return this.list[index]; }
        }

        /// <summary>
        /// Returns a new instance created by the specified XML string. String should have been created by <see cref="ConvertToXml"></see>.
        /// </summary>
        public static RecentCollection CreateFromXml(string xml)
        {
            List<RecentItem> list = SerializationTools.Deserialize<List<RecentItem>>(xml);
            if (list == null)
            {
                list = new List<RecentItem>();
            }

            RecentCollection newObject = new RecentCollection();
            newObject.list = list;

            return newObject;
        }

        /// <summary>
        /// Adds the given item into recent list and remove the last item in list, if the size exceeds maximum <see cref="MaxSize"></see>. If item already exists it is moved to top.
        /// </summary>
        public void PushItem(RecentItem item)
        {
            lock (this.syncObject)
            {
                foreach (RecentItem existingItem in this)
                {
                    if (existingItem.Equals(item))
                    {
                        if (this.ItemRemoved != null)
                        {
                            this.ItemRemoved(this, new RecentItemActionEventArgs(existingItem));
                        }

                        this.list.Remove(existingItem);
                        break;
                    }
                }

                this.list.Insert(0, item);
                if (this.ItemPushed != null)
                {
                    this.ItemPushed(this, new RecentItemActionEventArgs(item));
                }

                if (this.list.Count > this.MaxSize)
                {
                    if (this.ItemRemoved != null)
                    {
                        this.ItemRemoved(this, new RecentItemActionEventArgs(this.list[this.list.Count - 1]));
                    }

                    this.list.RemoveAt(this.list.Count - 1);
                }
            }
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        public void RemoveItem(RecentItem item)
        {
            if (this.ItemRemoved != null)
            {
                this.ItemRemoved(this, new RecentItemActionEventArgs(item));
            }

            this.list.Remove(item);
        }

        /// <summary>
        /// Gets the an XML string representation of the current instance.
        /// </summary>
        public string ConvertToXml()
        {
            return SerializationTools.Serialize(this.list);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        IEnumerator<RecentItem> IEnumerable<RecentItem>.GetEnumerator()
        {
            if (this.list == null)
            {
                return null;
            }
            else
            {
                return this.list.GetEnumerator();
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            if (this.list == null)
            {
                return null;
            }
            else
            {
                return this.list.GetEnumerator();
            }
        }
    }
}