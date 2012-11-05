using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ResEx.Win;
using System.Diagnostics;

namespace ResEx
{
    public partial class NavigationBar : UserControl
    {
        private TreeNavigator _TreeView;
        private string _separator = ">";

        [Description("The TreeView associated with this navigator control"), TypeConverter("System.Windows.Forms.TreeView"), DefaultValue(null)]
        public TreeNavigator TreeView 
        {
            get
            {
                return this._TreeView;
            }
            set
            {
                if (this._TreeView != null)
                {
                    this._TreeView.AfterSelect -= new EventHandler<TreeViewEventArgs>(TreeView_AfterSelect);
                }

                this._TreeView = value;
                this._TreeView.AfterSelect += new EventHandler<TreeViewEventArgs>(TreeView_AfterSelect);
            }
        }
            
        [Description("The separator character"), DefaultValue(">")]
        public string Separator
        {
            get
            {
                return this._separator;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Separator");
                }

                this._separator = value;
            }
        }

        public NavigationBar()
        {
            InitializeComponent();
        }

        private void SetPath(string path)
        {
            if (String.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            if (String.IsNullOrEmpty(Separator)) throw new ArgumentNullException("separator");

            FlowLayoutPanel1.SuspendLayout();

            try 
            {            
                // remove all items
                for (int i = FlowLayoutPanel1.Controls.Count - 1; i >= 0; i--)
                {            
                    Control control = FlowLayoutPanel1.Controls[i];
                    if (control is LinkLabel)
                        (control as LinkLabel).LinkClicked -= new LinkLabelLinkClickedEventHandler(LinkLabel_LinkClicked);
                    FlowLayoutPanel1.Controls.Remove(control);
                    control.Dispose();
                }


                // add items
                string[] spl = path.Split(this._TreeView.PathSeparator.ToCharArray());
                string t = string.Empty;

                for (int i = 0; i <= spl.Length - 1; i++)
                {
                    // add a separator label
                    if (i > 0)
                    {
                        Label label = new Label();
                        label.Text = this._separator;
                        label.Margin = new Padding(0);
                        label.AutoSize = true;
                        FlowLayoutPanel1.Controls.Add(label);
                        t += this._TreeView.PathSeparator;
                    }

                    // add link
                    t += spl[i];
                    Label link;
                    if (i == spl.Length - 1)
                        link = new Label();
                    else
                    {
                        link = new LinkLabel();
                        (link as LinkLabel).LinkClicked += LinkLabel_LinkClicked;
                        link.Tag = t;
                    }
                    link.Text = spl[i];
                    link.Margin = new Padding(0);
                    link.AutoSize = true;
                    FlowLayoutPanel1.Controls.Add(link);
                }
            }
            finally
            {
                FlowLayoutPanel1.ResumeLayout();
            }
        }

        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel link = (sender as LinkLabel);
            string path = link.Tag.ToString();

            if (this._TreeView != null)
            {
                TreeNode node = FindNode(this._TreeView.Nodes, path);
                if (node != null) this._TreeView.SelectedNode = node;
            }
        }
        
        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SetPath(e.Node.FullPath);
        }

        private static TreeNode FindNode(TreeNodeCollection Nodes, String path)
        {
            foreach (TreeNode node in Nodes)
            {
                if (String.Compare(node.FullPath, path) == 0)
                {
                    return node;
                }

                TreeNode res = FindNode(node.Nodes, path);
                
                if (res != null)
                {
                    return res;
                }
            }
            return null;
        }

        [DebuggerStepThrough]
        private void FlowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Gray, 0, 0, Width - 1, Height);
        }
    }
}
