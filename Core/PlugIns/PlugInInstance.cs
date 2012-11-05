using System;

namespace ResEx.Core.PlugIns
{
    public class PlugInInstance
    {
        public Type ClassType { get; set; }

        public PlugInAttribute Info { get; set; }

        public IPlugIn Instance { get; set; }
    }
}