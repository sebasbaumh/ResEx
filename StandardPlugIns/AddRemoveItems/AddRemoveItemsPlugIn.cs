using System;
using System.Windows.Forms;
using ResEx.Core;
using ResEx.Core.PlugIns;
using ResEx.Win;

namespace ResEx.StandardPlugIns.AddRemoveItems
{
    [PlugIn(Name = "Add/Remove Items",
            Author = "Dimitris Papadimitriou",
            Description = "Allows adding or removing resouce items from resource sets",
            Disabled = false)]
    public class AddRemoveItemsPlugIn : IPlugIn
    {
        private IContext context;
        private ButtonInfo addButton;
        private ButtonInfo removeButton;
        private ButtonInfo editButton;
        private const string WarningMessage = "Editing content of base resource set may cause serious problems to application using the resources!";

        public void Initialize(IContext context)
        {
            this.context = context;

            // create the button that will invoke adding items
            this.addButton = new ButtonInfo();
            this.addButton.Caption = "Add Resource Item";
            this.addButton.Image = Properties.Resources.resStringAdd;
            this.addButton.OnClick += this.AddButton_Click;
            this.addButton.ToolBarVisible = false;
            this.addButton.ToolTip = "Adds a new resource item to the current resource bundle";
            this.addButton.Context = PluginContext.ResourceSet.ToString(); // this button can be pressed when a local resource set is loaded/selected
            this.context.AddButton(this.addButton);

            // create the button that will invoke removing items
            this.removeButton = new ButtonInfo();
            this.removeButton.Caption = "Remove Resource Item";
            this.removeButton.Image = Properties.Resources.resStringDel;
            this.removeButton.OnClick += this.RemoveButton_Click;
            this.removeButton.ToolBarVisible = false;
            this.removeButton.ToolTip = "Removes selected resource item from the current resource bundle";
            this.removeButton.Context = PluginContext.ResourceItem.ToString();    // this button can be pressed when a row is selected on editing grid
            this.context.AddButton(this.removeButton);

            // create the button that will invoke editing items
            this.editButton = new ButtonInfo();
            this.editButton.Caption = "Edit Resource Item";
            this.editButton.OnClick += this.EditButton_Click;
            this.editButton.ToolBarVisible = false;
            this.editButton.ToolTip = "Edits selected resource item";
            this.editButton.Context = PluginContext.ResourceItem.ToString();    // this button can be pressed when a row is selected on editing grid
            this.context.AddButton(this.editButton);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new AddResourceItemForm())
                {
                    form.Item = new ResourceStringItem();
                    form.CurrentBaseResourceSet = this.context.CurrentBaseResourceSet;
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        this.context.CurrentBaseResourceSet.Add(form.Item.Name, form.Item);
                        this.context.RefreshCurrentBundle();
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.ShowExceptionMessageBox(ex);
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            try
            {
                var currentResourceItemKey = this.context.CurrentResourceItemKey;

                if (MessageBox.Show(string.Format("About to delete '{0}' from resource bundle!\r\n{1}\r\n\r\nAre you sure?", currentResourceItemKey, WarningMessage), Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK) return;

                // delete current resource item key from all resource sets
                foreach (var resourceSet in this.context.CurrentResourceBundle)
                {
                    if (resourceSet.ContainsKey(currentResourceItemKey))
                    {
                        resourceSet.Remove(currentResourceItemKey);
                    }
                }

                this.context.RefreshCurrentBundle();
            }
            catch (Exception ex)
            {
                Tools.ShowExceptionMessageBox(ex);
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(string.Format("About to edit '{0}'!\r\n{1}\r\n\r\nAre you sure?", this.context.CurrentResourceItemKey, WarningMessage), Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK) return;

                using (var form = new AddResourceItemForm())
                {
                    form.Item = (ResourceStringItem)this.context.CurrentBaseResourceSet[this.context.CurrentResourceItemKey];
                    form.CurrentBaseResourceSet = this.context.CurrentBaseResourceSet;
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        this.context.RefreshCurrentBundle();
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.ShowExceptionMessageBox(ex);
            }
        }
    }
}