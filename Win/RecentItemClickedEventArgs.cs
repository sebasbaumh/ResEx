using System;
using ResEx.Common;

namespace ResEx.Win
{
    public class RecentItemClickedEventArgs : EventArgs
    {
        public RecentItemClickedEventArgs(RecentItem recentItem)
        {
            this.RecentItem = recentItem;
        }

        public RecentItem RecentItem { get; private set; }
    }
}