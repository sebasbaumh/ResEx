using System.Collections.Generic;

namespace ResEx.Common
{
    /// <summary>
    /// Represents a generic collection of key/value pairs. Implements all functionality of a standard System.Collections.Generic.Dictionary and additionaly allows retrieving a value based on it's index.
    /// </summary>
    public interface IIndexedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        TValue this[int index]
        {
            get; 
            set;
        }
    }
}