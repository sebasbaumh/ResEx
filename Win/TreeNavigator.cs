using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ResEx.Win
{
    public partial class TreeNavigator : UserControl
    {
        public event EventHandler<TreeViewEventArgs> AfterSelect;

        public TreeNavigator()
        {
            this.InitializeComponent();

            this.Paint += this.TreeNavigator1_Paint;
            this.TreeView1.AfterSelect += this.TreeView1_AfterSelect;
        }

        [DebuggerStepThrough]
        private void TreeNavigator1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Gray, 0, 0, Width - 1, Height - 1);
        }

        public TreeNodeCollection Nodes
        {
            get { return this.TreeView1.Nodes; }
        }

        public TreeNode SelectedNode
        {
            get { return this.TreeView1.SelectedNode; }
            set { this.TreeView1.SelectedNode = value; }
        }

        public string PathSeparator
        {
            get { return this.TreeView1.PathSeparator; }
            set { this.TreeView1.PathSeparator = value; }
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.AfterSelect != null)
            {
                this.AfterSelect(this, e);
            }
        }
    }
}
