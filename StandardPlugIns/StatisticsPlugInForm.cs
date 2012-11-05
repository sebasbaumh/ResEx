using System.Windows.Forms;
using ResEx.Core;

namespace ResEx.StandardPlugIns
{
    public partial class StatisticsPlugInForm : Form
    {
        public StatisticsPlugInForm(IContext context)
        {
            this.InitializeComponent();

            this.ProjectNameLabel.Text = context.CurrentBaseFile;
        }
    }
}
