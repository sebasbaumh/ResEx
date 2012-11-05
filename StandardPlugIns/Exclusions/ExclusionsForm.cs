using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ResEx.Core;

namespace ResEx.StandardPlugIns.Exclusions
{
    public partial class ExclusionsForm : Form
    {
        public ExclusionsForm(IList<Exclusion> exclusions)
        {
            InitializeComponent();

            this.dataGridView1.DataSource = new BindingList<Exclusion>(exclusions);
        }
    }
}
