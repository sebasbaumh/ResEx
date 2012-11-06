using System;
using System.Linq;
using System.Windows.Forms;
using ResEx.Core;
using ResEx.Core.PlugIns;

namespace ResEx.StandardPlugIns
{
    [PlugIn(Name = "Copy Base Values",
            Author = "Dimitris Papadimitriou",
            Description = "Copies all values from base to local resource set",
            Disabled = false)]
    public class CopyBaseValuesPlugIn : IPlugIn
    {
        private IContext context;

        private ButtonInfo button;

        public void Initialize(IContext context)
        {
            this.context = context;
            
            // create the button that will invoke the plug in
            this.button = new ButtonInfo();
            this.button.Caption = "Copy all Base Values";
            this.button.Image = null;
            this.button.OnClick += this.Invoke;
            this.button.ToolBarVisible = false;
            this.button.ToolTip = "Copies all values from base to local resource set";
            this.button.Context = PluginContext.ResourceSet.ToString();    // this plug in can be invoke when a local resource set is loaded/selected
            this.context.AddButton(this.button);
        }

        private void Invoke(object sender, EventArgs e)
        {
            BaseValuesFilter filter;

            switch (MessageBox.Show("Warning! This will copy all base values as translated!\r\n\r\nSelect Yes to copy all values, No to copy not translated values only or Cancel to abort...", Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
            {
                case DialogResult.Cancel:
                    return;
                case DialogResult.Yes:
                    filter = BaseValuesFilter.AllUnlocked;
                    break;
                case DialogResult.No:
                    filter = BaseValuesFilter.NotTranslatedAndUnlocked;
                    break;
                default:
                    throw new InvalidOperationException("Unhandled message box result");
            }

            CopyResourceSetContent(this.context.CurrentBaseResourceSet, this.context.CurrentLocalResourceSet, filter);
        }

        /// <summary>
        /// Copies all values from source to target resource set
        /// </summary>
        private static void CopyResourceSetContent(ResourceSet source, ResourceSet target, BaseValuesFilter filter)
        {
            foreach (var sourceResourceItem in source.Values.Where(p => !p.Locked))
            {
                if (!target.ContainsKey(sourceResourceItem.Name))
                {
                    var targetResourceItem = new ResourceItem();
                    targetResourceItem.Name = sourceResourceItem.Name;
                    targetResourceItem.Value = sourceResourceItem.Value;
                    target.Add(sourceResourceItem.Name, targetResourceItem);
                }
                else
                {
                    if (target[sourceResourceItem.Name].ValueEmpty || filter == BaseValuesFilter.AllUnlocked)
                    {
                        target[sourceResourceItem.Name].Value = sourceResourceItem.Value;
                    }
                }
            }
        }
    }
}