using System;
using System.Reflection;
using System.Windows.Forms;

namespace ResEx
{
    internal partial class AboutBox : Form
    {
        private bool activatedFlag;

        public AboutBox()
        {
            InitializeComponent();

            this.Load += this.AboutBox_Load;
        }

        private void AboutBox_Load(object sender, EventArgs e)
        {
            this.Text = String.Format("About {0}", this.AssemblyTitle);
            this.LabelProductName.Text = this.AssemblyProduct;
            this.LabelVersion.Text = String.Format("Version {0}", this.AssemblyVersion);
            this.LabelCopyright.Text = this.AssemblyCopyright;
            this.CompanyNameLinkLabel.Text = this.AssemblyCompany;
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != string.Empty)
                    {
                        return titleAttribute.Title;
                    }
                }

                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }

                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }

                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }

                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }

                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        #endregion

        private void AboutBox_Activated(object sender, EventArgs e)
        {
            if (this.activatedFlag)
            {
                return;
            }

            this.activatedFlag = true;
            System.Media.SystemSounds.Exclamation.Play();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CompanyNameLinkLabel_Click(object sender, EventArgs e)
        {
            Main.OpenURL(Main.PapadiGr);
        }

        private void LicenseLinkLabel_Click(object sender, EventArgs e)
        {
            Main.OpenURL(Main.LicenseUrl);
        }
    }
}
