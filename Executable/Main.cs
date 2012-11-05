using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using ResEx.Win;

namespace ResEx
{
    public static class Main
    {
        public const string UpdateCheckURL = "http://resex.codeplex.com/Project/ProjectRss.aspx?ProjectRSSFeed=codeplex://release/resex";
        public const string ForumsURL = "http://resex.codeplex.com/Thread/List.aspx";
        public const string ResExHomePage = "http://resex.codeplex.com";
        public const string PapadiGr = "http://www.papadi.gr";
        public const string LicenseUrl = "http://resex.codeplex.com/license";
        public static readonly DateTime ReleaseDate = new DateTime(2010, 04, 20);

        public static void OpenURL(string url)
        {
            Process process = new Process();
            process.StartInfo.FileName = url;
            process.StartInfo.UseShellExecute = true;
            try 
            {
                process.Start();
            }
            catch
            {
            }
        }

        public static void CheckForUpdates(bool silent)
        {
            try
            {
                // if updates exist
                var checkResult = UpdateChecker.Check(UpdateCheckURL, true, ReleaseDate);
                if (checkResult != UpdateChecker.UpdateCheckResult.Nothing)
                {
                    string message = string.Format(My.Resources.Local.NewVersionAvailable, Tools.GetAssemblyProduct());
                    if (MessageBox.Show(message, Tools.GetAssemblyProduct(), MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        OpenURL(ResExHomePage);
                    }
                }
                else
                {
                    if (!silent)
                    {
                        MessageBox.Show(My.Resources.Local.YouHaveTheLatestVersion, Tools.GetAssemblyProduct(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception)
            {
                if (!silent)
                {
                    throw;
                }
            }
        }

        public static Version GetAssemblyVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}
