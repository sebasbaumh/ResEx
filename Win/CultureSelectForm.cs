using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace ResEx.Win
{
    public partial class CultureSelectForm : Form
    {
        public CultureInfo SelectedCulture { get; set; }

        public CultureSelectForm()
        {
            this.InitializeComponent();

            this.PromptLabel.Text = "Please select the culture you would like to add and start translating into...";
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            this.SelectedCulture = (CultureInfo)this.ComboBoxCultures.SelectedItem;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void CultureSelectForm_Load(object sender, EventArgs e)
        {
            this.SelectedCulture = Thread.CurrentThread.CurrentCulture;

            this.ComboBoxCultures.ValueMember = "Name";
            this.ComboBoxCultures.DisplayMember = "EnglishName";

            this.ComboBoxCultures.Items.Clear();
            foreach (var cultureInfo in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                this.ComboBoxCultures.Items.Add(cultureInfo);
            }

            // select the default culture (ignores any exceptions)
            try
            {
                this.ComboBoxCultures.SelectedItem = this.SelectedCulture;
            }
            catch
            {
            }
        }
    }
}
