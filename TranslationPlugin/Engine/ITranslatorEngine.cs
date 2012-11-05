using System;
using System.Collections.Generic;
using ResEx.Core;

namespace ResEx.TranslationPlugin.Engine
{
    public interface ITranslatorEngine
    {
        event EventHandler<TranslationReceivedEventArgs> TranslationReceived;

        void Translate(IEnumerable<AutoTranslationItem> translationItems, string from, string to);

        IAsyncResult BeginTranslate(IEnumerable<AutoTranslationItem> translationItems, string from, string to, AsyncCallback callback, object state);

        void Stop();

        void EndTranslate(IAsyncResult result);

        string DetectLanguage(IEnumerable<AutoTranslationItem> translationItems);

        Language[] GetLanguages();
    }
}