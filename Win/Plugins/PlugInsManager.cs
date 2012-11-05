using System;
using System.Collections.Generic;
using System.Diagnostics;
using ResEx.Common;
using ResEx.Core;
using ResEx.Core.PlugIns;

namespace ResEx.Win.PlugIns
{
    public sealed class PlugInsManager : IDisposable
    {
        public IEnumerable<PlugInInstance> PlugIns { get; internal set; }

        public PlugInsManager(IContext context)
        {
            var plugInsPath = StringTools.GetTypeFolder(this.GetType());
            this.PlugIns = Tools.DiscoverPlugIns(plugInsPath);

            foreach (var plugInInstance in this.PlugIns)
            {
                // create instance
                try
                {
                    plugInInstance.Instance = (IPlugIn)context.Container.Resolve(plugInInstance.ClassType);
                }
                catch (Exception e)
                {
                    // TODO : LOG ERROR TO FILE
                    Debug.Write("Failed to create plug in instance.\r\n" + e);
                    continue;
                }

                // initialize
                try
                {
                    plugInInstance.Instance.Initialize(context);
                }
                catch (Exception e)
                {
                    // TODO : LOG ERROR TO FILE
                    Debug.Write("Failed to initialize plug in.\r\n" + e);
                    continue;
                }
            }
        }

        #region IDisposable Implementation
        
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PlugInsManager() 
        {
            this.Dispose(false);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        private void Dispose(bool disposing)
        {
            if (disposing) 
            {
                // free managed resources
                if (this.PlugIns != null)
                {
                    // dispose all plug ins that implement the IDisposable interface
                    foreach (var plugInInstance in this.PlugIns)
                    {
                        var disposablePlugIn = plugInInstance.Instance as IDisposable;
                        if (disposablePlugIn != null)
                        {
                            disposablePlugIn.Dispose();
                        }
                    }
                }
            }

            //// free native resources if there are any.
        }

        #endregion
    }
}