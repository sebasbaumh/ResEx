using System.ComponentModel;
using ResEx.Common;

namespace ResEx.Core
{
    /// <summary>
    /// Represents an entry in a resource file.
    /// </summary>
    public class ResourceItem : INotifyPropertyChanged
    {
        #region Backing fields or properties

        //// IMPORTANT do not access backing fields directly
        //// Access them ONLY using the corresponding properties

        private string name;

        private string comment;

        private object value;

        private bool reviewPending;

        #endregion

        /// <summary>
        /// Gets or sets the unique identifier of this item
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value; 
                this.InvokePropertyChanged("Name");
            }
        }

        /// <summary>
        /// Gets or sets the comment to help the translator do the translation.
        /// This comment is clear of symbols that define resource item metadata, such as # for locked.
        /// </summary>
        public string Comment
        {
            get
            {
                return this.comment;
            }

            set
            {
                this.comment = value;
                this.InvokePropertyChanged("Comment");
            }
        }

        /// <summary>
        /// Gets or sets the value of the resource item
        /// </summary>
        public object Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value; 
                this.InvokePropertyChanged("Value");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current item must be reviewed. Among others
        /// this can be set to true by the web translator in order to allow human translator to review
        /// automatic translation.
        /// </summary>
        public bool ReviewPending
        {
            get
            {
                return this.reviewPending;
            }

            set
            {
                this.reviewPending = value;
                this.InvokePropertyChanged("ReviewPending");
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Value"/> is empty
        /// </summary>
        public bool ValueEmpty
        {
            get { return StringTools.ValueNullOrEmpty(this.Value); }
        }

        /// <summary>
        /// Gets a value indicating whether the resource item is locked.
        /// This means that it cannot be translated. This is useful for resource items
        /// that are not translatable (such as numbers, brand names etc.)
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// Gets the reason the item is locked.
        /// </summary>
        public LockedReason LockedReason { get; set; }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void InvokePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler changed = this.PropertyChanged;
            if (changed != null)
            {
                changed(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}