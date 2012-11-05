using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ResEx.Common;
using ResEx.Core;
using ResEx.StandardPlugIns.Exclusions;
using ResEx.TranslationPlugin.Engine;
using Tools = ResEx.Win.Tools;

namespace ResEx.TranslationPlugin
{
    public partial class TranslationForm : Form
    {
        public TranslationForm()
        {
            InitializeComponent();
        }

        public TranslationForm(IContext context, WebTranslatorPlugIn webTranslatorPlugIn) : this()
        {
            this.context = context;
            this.webTranslatorPlugIn = webTranslatorPlugIn;
        }

        private readonly IContext context;
        private readonly WebTranslatorPlugIn webTranslatorPlugIn;
        private bool running;
        private ITranslatorEngine translator;

        private void TranslationForm_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                // get languages list from translator service
                var translator = this.context.Container.Resolve<ITranslatorEngine>();
                var languages = translator.GetLanguages();
                var languages2 = new List<Language>(languages); // 2nd copy for 2nd combo box. cannot use the same instace for both else selected items get synchronized

                // fill combo boxes with languages
                comboBox1.DataSource = languages;
                comboBox1.DisplayMember = "DisplayName";
                comboBox1.ValueMember = "Code";
                comboBox2.DataSource = languages2.ToArray();
                comboBox2.DisplayMember = "DisplayName";
                comboBox2.ValueMember = "Code";

                // fill other combo box with static values
                var list3 = new List<KeyValuePair<int, string>>();
                list3.Add(new KeyValuePair<int, string>(0, "All Unlocked"));
                list3.Add(new KeyValuePair<int, string>(1, "Not Translated and Unlocked"));
                comboBox3.DataSource = list3;
                comboBox3.DisplayMember = "Value";
                comboBox3.ValueMember = "Key";
                comboBox3.SelectedIndex = 1;

                // select source language in combo box (use base resource language or english if not set - if english is not supported by translation engine then 1st will remain selected without any notice)
                string sourceCulture = this.context.CurrentBaseResourceSet.Culture;
                SetCombo(comboBox1, sourceCulture != ResourceSet.NeutralCulture ? sourceCulture : "en");

                // select target language in combo box
                string targetCulture = this.context.CurrentLocalResourceSet.Culture;
                SetCombo(comboBox2, targetCulture);
                if (comboBox2.SelectedValue == null)
                {
                    MessageBox.Show("Target culture couldn't be found. English selected as default");
                    comboBox2.SelectedValue = "en";
                }
            }
        }

        private static void SetCombo(ListControl combo, string culture)
        {
            combo.SelectedValue = culture;

            // best match
            if (combo.SelectedValue == null)
            {
                combo.SelectedValue = culture.Split('-')[0];
            }
        }

        private void TranslateButton_Click(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;

            try
            {
                if (!running)
                {
                    var sourceLanguage = (string)comboBox1.SelectedValue;
                    var targetLanguage = (string)comboBox2.SelectedValue;

                    var values = GetValuesForTranslation(this.context.CurrentBaseResourceSet).ToList();

                    // filter out already translated items, unless opposite is selected by end-user
                    var translateAll = (int)comboBox3.SelectedValue == 0;
                    var filteredValues = from p in values
                                         where !this.context.CurrentLocalResourceSet.HasValue(p.Text) || translateAll
                                         select p;

                    // invoke 'before translation hook' to allow other plugins to tamper with the translation
                    foreach (var translationItem in filteredValues)
                    {
                        this.webTranslatorPlugIn.InvokeBeforeItemAutoTranslation(translationItem);
                    }

                    translator = this.context.Container.Resolve<ITranslatorEngine>();
                    translator.TranslationReceived += this.translator_TranslationReceived;

                    this.running = true;
                    this.RefreshStatus();

                    translator.BeginTranslate(values, sourceLanguage, targetLanguage, this.TranslationComplete, null);
                }
                else
                {
                    this.translator.Stop();
                }
            }
            catch (Exception exception)
            {
                Tools.ShowExceptionMessageBox(exception);
            }
            finally
            {
                this.UseWaitCursor = false;
            }
        }

        private void TranslationComplete(IAsyncResult ar)
        {
            this.Invoke(new System.Threading.ThreadStart(() =>
                                                             {
                                                                 try
                                                                 {
                                                                     this.translator.EndTranslate(ar);
                                                                     MessageBox.Show("Translation Done!");
                                                                     this.Close();
                                                                 }
                                                                 catch (Exception exception)
                                                                 {
                                                                     Tools.ShowExceptionMessageBox(exception);
                                                                 }
                                                             }));
        }

        private void RefreshStatus()
        {
            this.InputPanel.Enabled = !this.running;
            this.btnTranslate.Text = this.running ? "&Stop" : "&Translate";
        }

        private static IEnumerable<AutoTranslationItem> GetValuesForTranslation(ResourceSet sourceResourceSet)
        {
            return from p in sourceResourceSet
                   where !p.Value.Locked && p.Value is ResourceStringItem && !StringTools.ValueNullOrEmpty(p.Value.Value)
                   select new AutoTranslationItem
                              {
                                  Key = p.Key,
                                  Text = (string)p.Value.Value
                              };
        }

        private void translator_TranslationReceived(object sender, TranslationReceivedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<TranslationReceivedEventArgs>(this.translator_TranslationReceived), new[] { sender, e });
            }
            else
            {
                try
                {
                    var resourceSet = this.context.CurrentLocalResourceSet;

                    foreach (var translationResult in e.Results)
                    {
                        // invoke 'after translation hook' to allow other plugins to tamper with the translation
                        this.webTranslatorPlugIn.InvokeAfterItemAutoTranslation(translationResult);

                        // TODO : HANDLE translationResult.Error
                        resourceSet.GetStringItem(translationResult.Key).Value = translationResult.Text;

                        if (this.markForReviewCheckBox.Checked)
                        {
                            resourceSet[translationResult.Key].ReviewPending = true;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Tools.ShowExceptionMessageBox("Failed to use translation results", exception);
                }                
            }
        }

        private void DetectButton_Click(object sender, EventArgs e)
        {
            try
            {
                var translator = this.context.Container.Resolve<ITranslatorEngine>();
                var language = translator.DetectLanguage(GetValuesForTranslation(this.context.CurrentBaseResourceSet));
                SetCombo(comboBox1, language);
            }
            catch (Exception exception)
            {
                Tools.ShowExceptionMessageBox(exception);
            }
        }

        private void ExclusionButton_Click(object sender, EventArgs e)
        {
            try
            {
                var form = new ExclusionsForm(this.context.CurrentBaseResourceSet.Exclusions);
                form.ShowDialog(this);
            }
            catch (Exception exception)
            {
                Tools.ShowExceptionMessageBox(exception);
            }
        }
    }
}
