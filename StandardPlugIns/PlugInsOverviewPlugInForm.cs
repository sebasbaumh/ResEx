using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ResEx.Core.PlugIns;
using ResEx.StandardPlugIns.Properties;
using ResEx.Win.PlugIns;

namespace ResEx.StandardPlugIns
{
    public partial class PlugInsOverviewPlugInForm : Form
    {
        public PlugInsOverviewPlugInForm(IEnumerable<PlugInInstance> plugIns)
        {
            this.InitializeComponent();
            this.Icon = Resources.PlugInIcon;

            // update UI from collection
            this.ItemsListBox.Items.AddRange(plugIns.Select(p => p.ClassType.AssemblyQualifiedName).ToArray());
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ItemsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if nothing is selected
            if (this.ItemsListBox.SelectedIndex == -1)
            {
                this.InfoTextBox.Text = string.Empty;
                return;
            }

            string info;

            // show information about the selected plug in
            var type = Type.GetType(this.ItemsListBox.Text, false, true);

            if (type == null)
            {
                info = "Plug in not found!";
            }
            else
            {
                var infoAttribute = type.GetPlugInAttribute();

                if (infoAttribute == null)
                {
                    // if attribute not found then just show the type name
                    info = type.FullName;
                }
                else
                {
                    info = "{0}\r\nAuthor : {1}\r\nDescription : {2}";
                    info = string.Format(CultureInfo.InstalledUICulture, info, infoAttribute.Name, infoAttribute.Author, infoAttribute.Description);
                }
            }

            this.InfoTextBox.Text = info;
        }
    }
}