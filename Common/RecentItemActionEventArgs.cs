using System;

namespace ResEx.Common
{
    public class RecentItemActionEventArgs : EventArgs
    {
        public RecentItemActionEventArgs(RecentItem item)
        {
            this.Item = item;
        }

        public RecentItem Item { get; set; }
    }
}