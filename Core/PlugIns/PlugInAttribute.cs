using System;

namespace ResEx.Core.PlugIns
{
    /// <summary>
    /// Specifies information regarding a plug in
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class PlugInAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the user friendly name of the plug in
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the author/developer of the plug in
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the description of the plug in
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the plug in disabled.
        /// Disabled plug ins are ignored by the application as if they never existed.
        /// </summary>
        public bool Disabled { get; set; }
    }
}