using System;
using System.Drawing;
using System.Windows.Forms;

namespace ResEx.Core
{
    public class ButtonInfo
    {
        public ButtonInfo()
        {
            this.Visible = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the plug in will be added as toolbar button
        /// </summary>
        public bool ToolBarVisible { get; set; }

        /// <summary>
        /// Gets or sets the icon that will represent the plug in on toolbars and/or menus
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// Gets or sets the caption of the button
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the tool tip of the button
        /// </summary>
        public string ToolTip { get; set; }

        /// <summary>
        /// Gets or sets the delegate to call when the button is clicked
        /// </summary>
        public EventHandler OnClick { get; set; }

        /// <summary>
        /// Gets or sets the delegate to call when the state of the UI changes significantly enough
        /// and the button may need to update its state. For instance the plug in may need to be
        /// disabled when no project is loaded.
        /// </summary>
        public EventHandler OnRefresh { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the button is disabled
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the button is visible
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets a keyboard shortcut combination to be used to invoke the button
        /// </summary>
        public Keys ShortcutKeys { get; set; }

        /// <summary>
        /// Gets or sets the context in which the plugin can be used and will be enabled
        /// </summary>
        public string Context { get; set; }
    }
}