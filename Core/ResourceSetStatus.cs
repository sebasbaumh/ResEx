namespace ResEx.Core
{
    public enum ResourceSetStatus
    {
        /// <summary>
        /// Existing item with no changes since loading
        /// </summary>
        Unaffected = 0,

        /// <summary>
        /// New item which has not been saved yet
        /// </summary>
        New = 1,

        /// <summary>
        /// Item marked for deletion uppon save
        /// </summary>
        Deleted = 2,

        /// <summary>
        /// Existing item that has changes pending to be saved
        /// </summary>
        Updated = 3
    }
}