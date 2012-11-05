using System;
using ResEx.Core;
using ResEx.Core.PlugIns;

namespace ResEx.StandardPlugIns.Exclusions
{
    [PlugIn(Name = "Exclusions",
            Author = "Dimitris Papadimitriou",
            Description = "Handles a list of items that should be excluded from translation",
            Disabled = false)]
    public class ExclusionsPlugIn : IPlugIn
    {
        private IContext context;

        private ButtonInfo button;

        private ExclusionManager exclusionManager;

        public void Initialize(IContext context)
        {
            this.context = context;

            // subscribe to events that will do the actual exclusion from automated translation
            this.context.BeforeItemAutoTranslation += this.context_BeforeItemAutoTranslation;
            this.context.AfterItemAutoTranslation += this.context_AfterItemAutoTranslation;
            this.context.CurrentResourceBundleChanged += context_CurrentResourceBundleChanged;

            // create the button that will invoke the plug in
            this.button = new ButtonInfo();
            this.button.Caption = "Exclusions";
            this.button.Image = null;
            this.button.OnClick += this.Invoke;
            this.button.ToolBarVisible = false;
            this.button.ToolTip = "Handles a list of items that should be excluded from translation";
            this.button.Context = PluginContext.ResourceBundle.ToString();    // this plug in can be invoke when a local resource set is loaded/selected
            this.context.AddButton(this.button);
        }

        private void context_CurrentResourceBundleChanged(object sender, EventArgs e)
        {
            // create a manager that will manage exclusion patterns, based on the patterns defined in the current
            // resource bundle
            this.exclusionManager = new ExclusionManager();
            exclusionManager.ExclusionPatterns = this.context.CurrentBaseResourceSet.Exclusions;
        }

        /// <summary>
        /// Before translating an item, replace with placeholders all the texts that should not be exluded.
        /// the placeholders will be replaced back with the original text after the translation
        /// See <see cref="context_AfterItemAutoTranslation"/>
        /// </summary>
        private void context_BeforeItemAutoTranslation(object sender, AutoTranslationEventArgs<AutoTranslationItem> e)
        {
            // pass strings from exclusion manager to exclude items that should not be translated
            e.Item.Text = this.exclusionManager.Exclude(e.Item.Text);
        }

        /// <summary>
        /// Replaces placeholders - created by <see cref="context_BeforeItemAutoTranslation"/> -  back to original texts
        /// </summary>
        private void context_AfterItemAutoTranslation(object sender, AutoTranslationEventArgs<AutoTranslationResult> e)
        {
            e.Item.Text = this.exclusionManager.Restore(e.Item.Text);
        }

        private void Invoke(object sender, EventArgs e)
        {
            var form = new ExclusionsForm(this.context.CurrentBaseResourceSet.Exclusions);
            form.ShowDialog();
        }
    }
}