using System;

namespace ResEx.Core
{
    public class ResourceItemChangedEventArgs : EventArgs
    {
        public ResourceItem Item { get; set; }
    }
}