using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Syndication;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ResEx.Win
{
    public static class Tools
    {
        /// <summary>
        /// Displays a message box with the contents of given exception.
        /// Shows Message of exception and of inner exception.
        /// </summary>
        public static void ShowExceptionMessageBox(Exception ex)
        {
            ShowExceptionMessageBox(null, ex);
        }

        /// <summary>
        /// Displays a message box with the contents of given exception.
        /// Shows given message followed by Message of exception and of inner exception.
        /// </summary>
        public static void ShowExceptionMessageBox(string message, Exception ex)
        {
            // create the text to display (show up to one level of inner exception)
            var text = new StringBuilder();

            if (!string.IsNullOrEmpty(message))
            {
                text.AppendLine(message);
            }

            if (ex != null)
            {
                text.AppendLine(ex.Message);
            }

            if (ex != null && ex.InnerException != null)
            {
                text.AppendLine(ex.InnerException.Message);
            }

            // show the text
            MessageBox.Show(text.ToString(), GetAssemblyProduct(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static string GetAssemblyProduct()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (attributes.Length == 0)
            {
                return string.Empty;
            }

            return ((AssemblyProductAttribute)attributes[0]).Product;
        }
    }
}