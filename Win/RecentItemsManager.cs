using System;
using System.ComponentModel;
using System.Windows.Forms;
using ResEx.Common;

namespace ResEx.Win
{
    public partial class RecentItemsManager : Component
    {
        private RecentCollection recentItems;
        
        /// <summary>
        /// Gets or sets the index of the <see cref="FileToolStripMenuItem"/> that the recent items will be placed.
        /// </summary>
        public int RecentItemsMenuIndex { get; set; }

        /// <summary>
        /// Gets or sets the menu item where recent items will be displayed
        /// </summary>
        public ToolStripMenuItem FileToolStripMenuItem { get; set; }

        /// <summary>
        /// Gets or sets the separator of menu that will be visible when recent items exist
        /// in order to separate them from other menu items
        /// </summary>
        public ToolStripSeparator RecentItemsToolStripSeparator { get; set; }

        #region Event

        /// <summary>
        /// Raised when an recent item is clicked on the menu
        /// </summary>
        public event EventHandler<RecentItemClickedEventArgs> ItemClicked;

        private void InvokeItemClicked(RecentItem recentItem)
        {
            var handler = this.ItemClicked;
            if (handler != null)
            {
                handler(this, new RecentItemClickedEventArgs(recentItem));
            }
        }

        #endregion

        #region Constructors

        public RecentItemsManager()
        {
            this.InitializeComponent();

            this.RecentItemsMenuIndex = 1;
        }

        public RecentItemsManager(IContainer container)
        {
            container.Add(this);

            this.InitializeComponent();

            this.RecentItemsMenuIndex = 1;
        }

        #endregion

        /// <summary>
        /// Fills recent item list with given xml
        /// </summary>
        public void Fill(string recentItemsXml, int maxSize)
        {
            this.recentItems = RecentCollection.CreateFromXml(recentItemsXml);
            this.recentItems.MaxSize = maxSize;
            this.recentItems.ItemPushed += this.RecentItems_ItemPushed;
            this.recentItems.ItemRemoved += this.RecentItems_ItemRemoved;

            foreach (RecentItem item in this.recentItems)
            {
                this.RecentItemAdd(item);
            }
        }

        /// <summary>
        /// Returns an xml representation of the current items in recent list
        /// </summary>
        public string ConvertToXml()
        {
            if (this.recentItems != null)
            {
                return this.recentItems.ConvertToXml();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Removes given item from recent list
        /// </summary>
        public void RemoveItem(RecentItem item)
        {
            this.recentItems.RemoveItem(item);
        }

        /// <summary>
        /// Pushes given item from recent list by adding it on top other others.
        /// </summary>
        public void PushItem(RecentItem item)
        {
            this.recentItems.PushItem(item);
        }

        private void RecentItems_ItemPushed(object sender, RecentItemActionEventArgs e)
        {
            this.RecentItemAdd(e.Item);
        }

        /// <summary>
        /// Adds the given recent item as menu item
        /// </summary>
        private void RecentItemAdd(RecentItem item)
        {
            ToolStripMenuItem toolStripItem = new ToolStripMenuItem(item.Caption);
            toolStripItem.Click += this.RecentItemClicked;
            this.FileToolStripMenuItem.DropDownItems.Insert(this.RecentItemsMenuIndex, toolStripItem);
            item.MenuItem = toolStripItem;
            toolStripItem.Tag = item;
            this.RecentItemsToolStripSeparator.Visible = this.recentItems.Count > 0;
        }

        /// <summary>
        /// Raised when a recent item is removed from list.
        /// This is can be done by removing it explicitely or when the maximum items in recent
        /// list was reached and an old item must be removed.
        /// </summary>
        private void RecentItems_ItemRemoved(object sender, RecentItemActionEventArgs e)
        {
            this.FileToolStripMenuItem.DropDownItems.Remove((ToolStripItem)e.Item.MenuItem);
            this.RecentItemsToolStripSeparator.Visible = this.recentItems.Count > 0;
        }

        /// <summary>
        /// Invoked when an recent item is clicked by the end user.
        /// This method propagates the event to container.
        /// </summary>
        private void RecentItemClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            RecentItem recentItem = (RecentItem)item.Tag;
            this.InvokeItemClicked(recentItem);
        }
    }
}