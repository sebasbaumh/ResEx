using System;
using ResEx.Core;

namespace ResEx.TranslationPlugin.Engine
{
    public class TranslationReceivedEventArgs : EventArgs
    {
        public TranslationReceivedEventArgs(AutoTranslationResult[] results)
        {
            this.Results = results;
        }

        public AutoTranslationResult[] Results { get; private set; }
    }
}