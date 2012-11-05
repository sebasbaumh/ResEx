using System;
using System.Windows.Forms;
using ResEx.Core;
using ResEx.Core.PlugIns;

namespace ResEx.StandardPlugIns
{
    [PlugIn(Name = "Mark for reviewing",
                Author = "Dimitris Papadimitriou",
                Description = "Provides buttons that mark/unmark items for later reviewing",
                Disabled = false)]
    public class ReviewMarksPlugin : IPlugIn
    {
        private IContext context;

        private ButtonInfo markButton;

        private ButtonInfo unmarkButton;
        
        public void Initialize(IContext context)
        {
            this.context = context;

            // create the button to mark
            this.markButton = new ButtonInfo();
            this.markButton.Caption = "Mark for Review";
            this.markButton.Image = null;
            this.markButton.OnClick += this.MarkInvoke;
            this.markButton.OnRefresh += this.Refresh;
            this.markButton.ToolBarVisible = true;
            this.markButton.ToolTip = "Marks item for later review";
            this.markButton.ShortcutKeys = Keys.Control | Keys.M;
            this.markButton.Context = PluginContext.LocalResourceItem.ToString();    // this plug in can be invoke when a resouce item is selected
            this.context.AddButton(this.markButton);

            // create the button to unmark
            this.unmarkButton = new ButtonInfo();
            this.unmarkButton.Caption = "Unmark for Review";
            this.unmarkButton.Image = null;
            this.unmarkButton.OnClick += this.UnmarkInvoke;
            this.unmarkButton.OnRefresh += this.Refresh;
            this.unmarkButton.ToolBarVisible = true;
            this.unmarkButton.ToolTip = "Unmarks item for later review";
            this.unmarkButton.ShortcutKeys = Keys.Control | Keys.U;
            this.unmarkButton.Context = PluginContext.LocalResourceItem.ToString();    // this plug in can be invoke when a resouce item is selected
            this.context.AddButton(this.unmarkButton);
        }

        private void Refresh(object sender, EventArgs e)
        {
            var selectedItemExists = !string.IsNullOrEmpty(this.context.CurrentResourceItemKey);
            var markedForReview = false;
            var locked = false;

            if (selectedItemExists)
            {
                var currentKey = this.context.CurrentResourceItemKey;
                markedForReview = this.context.CurrentLocalResourceSet != null && this.context.CurrentLocalResourceSet.ContainsKey(currentKey) && this.context.CurrentLocalResourceSet[currentKey].ReviewPending;
                locked = this.context.CurrentBaseResourceSet[currentKey].Locked;
            }
            
            // set button visibility according to review state of selected item
            this.markButton.Visible = selectedItemExists && !markedForReview;
            this.unmarkButton.Visible = selectedItemExists && markedForReview;

            // disable buttons for locked items
            this.markButton.Disabled = !selectedItemExists || this.markButton.Disabled || locked;
            this.unmarkButton.Disabled = !selectedItemExists || this.markButton.Disabled || locked;
        }

        private void MarkInvoke(object sender, EventArgs e)
        {
            this.context.CurrentLocalResourceSet.GetStringItem(this.context.CurrentResourceItemKey).ReviewPending = true; 
        }

        private void UnmarkInvoke(object sender, EventArgs e)
        {
            this.context.CurrentLocalResourceSet[this.context.CurrentResourceItemKey].ReviewPending = false;
        }
    }
}
