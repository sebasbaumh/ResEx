using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using ResEx.Core.PlugIns;

namespace ResEx.Core
{
    public interface IContext : IAutoTranslationContext
    {
        /// <summary>
        /// Raised when the selected resource item on the user interface changes
        /// </summary>
        event EventHandler CurrentResourceItemChanged;

        /// <summary>
        /// Raised when the current resource bundle changes
        /// </summary>
        event EventHandler CurrentResourceBundleChanged;

        /// <summary>
        /// Gets the name of the current file opened
        /// </summary>
        string CurrentBaseFile { get; }

        /// <summary>
        /// Gets the current <see cref="ResourceBundle"/>
        /// </summary>
        ResourceBundle CurrentResourceBundle { get; }

        /// <summary>
        /// Gets the current <see cref="ResourceSet"/> that is subject for translation
        /// </summary>
        ResourceSet CurrentLocalResourceSet { get; }

        /// <summary>
        /// Gets the current <see cref="ResourceSet"/> that is used as base for the translation
        /// </summary>
        ResourceSet CurrentBaseResourceSet { get; }

        /// <summary>
        /// Refreshes user interface by reloading current bundle from memory
        /// </summary>
        void RefreshCurrentBundle();

        /// <summary>
        /// Gets the key of the resource item selected by the end user on the UI. Null if none selected.
        /// </summary>
        string CurrentResourceItemKey { get; }

        /// <summary>
        /// Gets a value indicating whether the current opened project has unsaved changes.
        /// </summary>
        bool CurrentProjectIsDirty { get; }

        /// <summary>
        /// Gets a list of loaded plugins
        /// </summary>
        IEnumerable<PlugInInstance> PlugIns { get; }

        /// <summary>
        /// Gets an instance of unity inversion of control container
        /// </summary>
        IUnityContainer Container { get;  }

        /// <summary>
        /// Adds a button to UI according to the given button configuration
        /// </summary>
        void AddButton(ButtonInfo buttonInfo);

        /// <summary>
        /// Initializes the current instance
        /// </summary>
        void Initialize();
    }
}