using System;
using System.Threading;
using ResEx.Core;
using ResEx.Core.PlugIns;
using ResEx.TranslationPlugin.Engine;

namespace ResEx.TranslationPlugin
{
    [PlugIn(Name = "Web Translator",
            Author = "Dimitris Papadimitriou, Predrag Tomasevic",
            Description = "Automatically translates resources using Microsoft Translator services",
            Disabled = false)]
    public class WebTranslatorPlugIn : IPlugIn, IAutoTranslationContext
    {
        private IContext context;

        private ButtonInfo button;

        public void Initialize(IContext context)
        {
            this.context = context;

            this.context.Container.RegisterType<ITranslatorEngine, MicrosoftTranslatorEngine>();
            
            // create the button that will invoke the plug in
            this.button = new ButtonInfo();
            this.button.Caption = "Web Translator";
            this.button.Image = null;
            this.button.OnClick += this.Invoke;
            this.button.ToolBarVisible = true;
            this.button.ToolTip = "Automatically translate resources using Google services";
            this.button.Context = PluginContext.ResourceSet.ToString();    // this plug in can be invoke when a local resource set is loaded/selected
            this.context.AddButton(this.button);

            // load translation languages in the background
            // this will significantly speed up opening the translation form
            ThreadPool.QueueUserWorkItem(LoadLanguages);
        }

        private void LoadLanguages(object state)
        {
            try
            {
                var translator = this.context.Container.Resolve<ITranslatorEngine>();
                translator.GetLanguages();
            }
            catch
            {
                // do nothing with the exception. it will be displayed to end user
                // the next time languages are explicitely requested
            }
        }

        private void Invoke(object sender, EventArgs e)
        {
            TranslationForm frmTranslation = new TranslationForm(this.context, this);
            frmTranslation.ShowDialog();
        }

        #region IAutoTranslationContext

        public event EventHandler<AutoTranslationEventArgs<AutoTranslationItem>> BeforeItemAutoTranslation;

        public event EventHandler<AutoTranslationEventArgs<AutoTranslationResult>> AfterItemAutoTranslation;

        internal void InvokeBeforeItemAutoTranslation(AutoTranslationItem item)
        {
            EventHandler<AutoTranslationEventArgs<AutoTranslationItem>> handler = BeforeItemAutoTranslation;
            if (handler != null) handler(this, new AutoTranslationEventArgs<AutoTranslationItem>(item));
        }

        internal void InvokeAfterItemAutoTranslation(AutoTranslationResult result)
        {
            EventHandler<AutoTranslationEventArgs<AutoTranslationResult>> handler = AfterItemAutoTranslation;
            if (handler != null) handler(this, new AutoTranslationEventArgs<AutoTranslationResult>(result));
        }

        #endregion
    }
}