using System;

namespace ResEx.Common
{
    public class RecentItem : IEquatable<RecentItem>
    {
        public RecentItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentItem" /> class.
        /// </summary>
        public RecentItem(string caption, string type, string id)
        {
            this.Caption = caption;
            this.ItemType = type;
            this.Id = id;
        }

        /// <summary>
        /// Gets or sets the menu item instance related to the current instance.
        /// </summary>
        /// <value>The menu item.</value>
        [System.Xml.Serialization.XmlIgnore]
        public object MenuItem { get; set; }

        /// <summary>
        /// Gets or sets a string that identifies the type of item this instance refers to. This could be the System.Type string of a form.
        /// </summary>
        /// <remarks> In combination with <see cref="Id"/> this can completely identifies the item related to this instance.</remarks>
        public string ItemType { get; set; }

        /// <summary>
        /// Gets or sets the caption of the recent item. This should be a user friendly description
        /// </summary>
        /// <value>The caption.</value>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the Id of the recent item. In combination with <see cref="ItemType"/> this can completely identifies the item related to this instance.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Returns if current instance refers to the same item as the specified RecentItem given as parameter.
        /// </summary>
        /// <param name="other">The other.</param>
        public bool Equals(RecentItem other)
        {
            if (other == null)
            {
                return false;
            }

            if (string.Compare(this.Id, other.Id, StringComparison.OrdinalIgnoreCase) == 0 & string.Compare(this.ItemType, other.ItemType, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}