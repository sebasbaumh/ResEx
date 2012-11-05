using System;
using System.Windows.Forms;
using ResEx.Core;

namespace ResEx.StandardPlugIns.AddRemoveItems
{
    public partial class AddResourceItemForm : Form
    {
        // item being edited or added
        public ResourceStringItem Item { get; set; }

        // base resource set the item (will) belong(s) to
        public ResourceSet CurrentBaseResourceSet { get; set; }

        public AddResourceItemForm()
        {
            InitializeComponent();

            this.Load += AddResourceItemForm_Load;
        }

        private void AddResourceItemForm_Load(object sender, EventArgs e)
        {
            this.Text = this.IsNew ? "Add New Resource Item" : "Edit Resource Item";
            Assign2UI();
        }

        private void Assign2UI()
        {
            this.KeyTextBox.Text = Item.Name;
            this.CommentTextBox.Text = Item.Comment;
            this.ValueTextBox.Text = (string)Item.Value;
            this.MaxLengthTextBox.Text = Item.MaxLength == ResourceStringItem.UnlimitedLength ? string.Empty : Item.MaxLength.ToString();
            this.LockedCheckBox.Checked = Item.Locked;
        }

        private void Assign2Object()
        {
            Item.Name = this.KeyTextBox.Text;
            Item.Comment = this.CommentTextBox.Text;
            Item.Value = this.ValueTextBox.Text;
            Item.MaxLength = string.IsNullOrEmpty(this.MaxLengthTextBox.Text) ? ResourceStringItem.UnlimitedLength : int.Parse(this.MaxLengthTextBox.Text);
            Item.Locked = this.LockedCheckBox.Checked;
        }

        private bool ValidateInput()
        {
            bool valid = true;
            errorProvider1.Clear();

            if (string.IsNullOrEmpty(this.KeyTextBox.Text))
            {
                errorProvider1.SetError(this.KeyTextBox, "Cannot be empty");
                valid = false;
            }

            if (string.IsNullOrEmpty(this.ValueTextBox.Text))
            {
                errorProvider1.SetError(this.ValueTextBox, "Cannot be empty");
                valid = false;
            }

            // validate that maximum length is greater than zero (empty means -1 which means no maximum length)
            int maxLength = 0;
            if (!string.IsNullOrEmpty(this.MaxLengthTextBox.Text) && (!int.TryParse(this.MaxLengthTextBox.Text, out maxLength) || maxLength < 1))
            {
                errorProvider1.SetError(this.MaxLengthTextBox, "Expecting possitive integer value. Leave empty for no maximum restriction.");
                valid = false;
            }

            // check if value length is greater than maximum (if maximum is set)
            if (maxLength != 0 && this.ValueTextBox.Text.Length > maxLength)
            {
                errorProvider1.SetError(this.ValueTextBox, "Value length cannot be greater than maximum length");
                valid = false;
            }

            if (this.IsNew && this.CurrentBaseResourceSet.ContainsKey(this.KeyTextBox.Text))
            {
                errorProvider1.SetError(this.KeyTextBox, "Key already exists");
                valid = false;
            }

            return valid;
        }

        private bool IsNew
        {
            get
            {
                return !this.CurrentBaseResourceSet.Values.Contains(this.Item);
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (this.ValidateInput())
            {
                this.Assign2Object();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
