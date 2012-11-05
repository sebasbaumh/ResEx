using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using ResEx.Core;
using ResEx.Core.PlugIns;
using ResEx.Win.PlugIns;

namespace ResEx
{
    public class Context : IContext, IDisposable
    {
        private readonly MainForm mainForm;

        private PlugInsManager PlugInsManager { get; set; }

        private Action<ButtonInfo> AddButtonDelegate { get; set; }

        public event EventHandler CurrentResourceItemChanged;

        public event EventHandler CurrentResourceBundleChanged;

        internal void InvokeCurrentResourceBundleChanged()
        {
            EventHandler handler = CurrentResourceBundleChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        internal void InvokeCurrentResourceItemChanged()
        {
            EventHandler handler = CurrentResourceItemChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public string CurrentBaseFile { get; set; }

        public ResourceBundle CurrentResourceBundle { get; set; }

        public ResourceSet CurrentLocalResourceSet { get; set; }

        public ResourceSet CurrentBaseResourceSet { get; set; }

        public void RefreshCurrentBundle()
        {
            this.mainForm.RefreshCurrentBundle();
        }

        /// <summary>
        /// Gets the key of the resource item selected by the end user on the UI
        /// </summary>
        public string CurrentResourceItemKey
        {
            get
            {
                var row = this.mainForm.CustomDataGrid1.CurrentRow;
                if (row == null)
                {
                    return null;
                }
                else
                {
                    return row.Cells[ColumnNames.Key].Value.ToString();
                }
            }
        }

        public bool CurrentProjectIsDirty { get; set; }

        public IEnumerable<PlugInInstance> PlugIns
        {
            get { return this.PlugInsManager.PlugIns; }
        }

        public IUnityContainer Container
        {
            get; private set;
        }

        public void AddButton(ButtonInfo buttonInfo)
        {
            this.AddButtonDelegate.Invoke(buttonInfo);
        }

        public void Initialize()
        {
            // instanciate and configure Unity IOC container
            this.Container = new UnityContainer();
            this.Container.RegisterInstance<IContext>(this);

            this.PlugInsManager = this.Container.Resolve<PlugInsManager>();

            // hook to events of plugins impelementing the IAutoTranslationContext interface
            var autoTranslationContextPlugins = from p in this.PlugIns where p.Instance is IAutoTranslationContext select (IAutoTranslationContext)p.Instance;
            foreach (var plugIn in autoTranslationContextPlugins)
            {
                plugIn.BeforeItemAutoTranslation += this.BeforeItemAutoTranslation;
                plugIn.AfterItemAutoTranslation += this.AfterItemAutoTranslation;
            }
        }

        public Context(Action<ButtonInfo> addButtonDelegate, MainForm mainForm)
        {
            if (addButtonDelegate == null)
            {
                throw new ArgumentNullException("addButtonDelegate");
            }

            this.AddButtonDelegate = addButtonDelegate;
            this.mainForm = mainForm;
        }

        #region IAutoTranslationContext

        public event EventHandler<AutoTranslationEventArgs<AutoTranslationItem>> BeforeItemAutoTranslation;

        public event EventHandler<AutoTranslationEventArgs<AutoTranslationResult>> AfterItemAutoTranslation;

        #endregion

        #region IDisposable Implementation
        
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Context() 
        {
            this.Dispose(false);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) 
            {
                // free managed resources
                this.PlugInsManager.Dispose();
            }

            //// free native resources if there are any.
        }

        #endregion
    }
}