using System.Collections.Generic;

namespace ResEx.TranslationPlugin.Engine
{
    public static class Tools
    {
        /// <summary>
        /// Returns an enumerable list of chunks of items of the given list
        /// </summary>
        public static IEnumerable<T[]> GetChunks<T>(this IEnumerable<T> items, int chunkSize)
        {
            var tempList = new List<T>();
            var counter = 0;

            foreach (var item in items)
            {
                counter++;
                tempList.Add(item);

                if (counter == chunkSize)
                {
                    yield return tempList.ToArray();

                    tempList.Clear();
                    counter = 0;
                }
            }

            // return the rest that didn't fill a chunk
            if (tempList.Count > 0)
            {
                yield return tempList.ToArray();
            }
        }
    }
}