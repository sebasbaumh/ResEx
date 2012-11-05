using System;

namespace ResEx.Core
{
    public class AutoTranslationEventArgs<T> : EventArgs
    {
        public AutoTranslationEventArgs(T item)
        {
            this.Item = item;
        }

        public T Item { get; private set; }
    }
}