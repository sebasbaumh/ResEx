namespace ResEx.Core
{
    /// <summary>
    /// Reasons for which a resource item may be locked, resulting in not being editable for the translator to translate.
    /// </summary>
    public enum LockedReason
    {
        /// <summary>
        /// Item is locked for unknown reason
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Developer choose to lock the item
        /// </summary>
        DeveloperLock = 1,

        /// <summary>
        /// Framework used to create the resource file considers the item locked (eg. VS add's >> for some items in forms' resource files)
        /// </summary>
        FrameworkLock = 2,

        /// <summary>
        /// Item contains metadata to be used by Resex and not translatable items
        /// </summary>
        ResexMetadata = 3
    }
}