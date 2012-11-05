using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ResEx.Core;
using ResEx.TranslationPlugin.MicrosoftTranslator;

namespace ResEx.TranslationPlugin.Engine
{
    public class MicrosoftTranslatorEngine : ITranslatorEngine
    {
        private const int ChunkSize = 10;

        private const string AppId = "D442ACA6EAAF8BEFE15824ACB467D01277EA5260";

        #region Events

        public event EventHandler<TranslationReceivedEventArgs> TranslationReceived;

        private void InvokeTranslationReceived(TranslationReceivedEventArgs e)
        {
            EventHandler<TranslationReceivedEventArgs> received = TranslationReceived;
            if (received != null) received(this, e);
        }

        #endregion

        #region Async Implementation

        private delegate void TranslateInvoker(IEnumerable<AutoTranslationItem> translationItems, string from, string to);

        private TranslateInvoker asyncWorkerInstance;

        private bool stopRequested;

        public IAsyncResult BeginTranslate(IEnumerable<AutoTranslationItem> translationItems, string from, string to, AsyncCallback callback, object state)
        {
            if (asyncWorkerInstance != null) throw new InvalidOperationException("Another asynchronous process is in progress");     // do not translate
            this.stopRequested = false;
            asyncWorkerInstance = new TranslateInvoker(Translate);
            return asyncWorkerInstance.BeginInvoke(translationItems, from, to, callback, state);
        }

        public void Stop()
        {
            if (asyncWorkerInstance == null) throw new InvalidOperationException("No asynchronous process is in progress"); // do not translate
            this.stopRequested = true;
        }

        public void EndTranslate(IAsyncResult result)
        {
            this.asyncWorkerInstance.EndInvoke(result);
            this.asyncWorkerInstance = null;
        }

        #endregion

        public string DetectLanguage(IEnumerable<AutoTranslationItem> translationItems)
        {
            // get first chunk of the given items
            var firstChunk = translationItems.GetChunks(ChunkSize).First();
            var firstChunkStrings = firstChunk.Select(p => p.Text).ToArray();

            // detect language for each of the given strings
            var translator = new LanguageServiceClient();
            var detectedLanguages = translator.DetectArray(AppId, firstChunkStrings);

            // keep the language that was detected mostly
            var language = (from p in detectedLanguages
                            group p by p into g
                            orderby g.Count() descending 
                            select new { Language = g.Key, Count = g.Count() }).First().Language;

            return language;
        }

        public void Translate(IEnumerable<AutoTranslationItem> translationItems, string from, string to)
        {
            if (translationItems == null) throw new ArgumentNullException("translationItems");
            if (string.IsNullOrEmpty(from)) throw new ArgumentNullException("from");
            if (string.IsNullOrEmpty(to)) throw new ArgumentNullException("to");

            var translator = new LanguageServiceClient();
            var options = new TranslateOptions();
            
            foreach (var translationItemsChunk in translationItems.GetChunks(ChunkSize))
            {
                var texts = from p in translationItemsChunk select p.Text.Length <= 2000 ? p.Text : p.Text.Substring(0, 2000);   // 2000 is used because this is the current limit of ms translation services

                // send for translation
                var textsArray = texts.ToArray();
                TranslateArrayResponse[] microsoftTranslatorResponses = translator.TranslateArray(AppId, textsArray, from, to, options);

                // convert the response into our TranslationResult array
                var results = new List<AutoTranslationResult>();
                var tempCounter = 0;
                foreach (var translationItem in translationItemsChunk)
                {
                    var microsoftTranslatorResponse = microsoftTranslatorResponses[tempCounter];
                    var translationResult = new AutoTranslationResult
                                                {
                                                    Error = microsoftTranslatorResponse.Error,
                                                    Key = translationItem.Key,
                                                    Text = microsoftTranslatorResponse.TranslatedText,
                                                    OriginalText = translationItem.Text
                                                };
                    results.Add(translationResult);

                    tempCounter++;
                }

                // invoke event with the translation
                this.InvokeTranslationReceived(new TranslationReceivedEventArgs(results.ToArray()));

                if (this.stopRequested) break;
            }
        }

        private static Language[] languages;
        private static readonly object languagesLoadingLock = new object();

        public Language[] GetLanguages()
        {
            lock (languagesLoadingLock)
            {
                if (languages == null)
                {
                    var translator = new LanguageServiceClient();
                    var languageNames = translator.GetLanguagesForTranslate(AppId);
                    var languageDisplaynames = translator.GetLanguageNames(AppId, Thread.CurrentThread.CurrentUICulture.Name, languageNames);

                    var result = new List<Language>();
                    for (int i = 0; i < languageNames.Length; i++)
                    {
                        result.Add(new Language
                        {
                            Code = languageNames[i],
                            DisplayName = languageDisplaynames[i]
                        });
                    }

                    languages = result.ToArray();
                }
            }

            return languages;
        }
    }
}