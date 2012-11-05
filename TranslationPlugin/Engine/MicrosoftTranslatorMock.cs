using System;
using ResEx.TranslationPlugin.MicrosoftTranslator;

namespace ResEx.TranslationPlugin.Engine
{
    public class MicrosoftTranslatorMock : LanguageService
    {
        public void AddTranslation(string appId, string originalText, string translatedText, string from, string to, int rating, string contentType, string category, string user, string uri)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginAddTranslation(string appId, string originalText, string translatedText, string from, string to, int rating, string contentType, string category, string user, string uri, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndAddTranslation(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public int[] BreakSentences(string appId, string text, string language)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginBreakSentences(string appId, string text, string language, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public int[] EndBreakSentences(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public string Detect(string appId, string text)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginDetect(string appId, string text, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public string EndDetect(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public string[] DetectArray(string appId, string[] texts)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginDetectArray(string appId, string[] texts, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public string[] EndDetectArray(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public string GetAppIdToken(string appId, int minRatingRead, int maxRatingWrite, int expireSeconds)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginGetAppIdToken(string appId, int minRatingRead, int maxRatingWrite, int expireSeconds, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public string EndGetAppIdToken(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public string[] GetLanguageNames(string appId, string locale, string[] languageCodes)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginGetLanguageNames(string appId, string locale, string[] languageCodes, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public string[] EndGetLanguageNames(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public string[] GetLanguagesForSpeak(string appId)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginGetLanguagesForSpeak(string appId, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public string[] EndGetLanguagesForSpeak(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public string[] GetLanguagesForTranslate(string appId)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginGetLanguagesForTranslate(string appId, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public string[] EndGetLanguagesForTranslate(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public GetTranslationsResponse GetTranslations(string appId, string text, string from, string to, int maxTranslations, TranslateOptions options)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginGetTranslations(string appId, string text, string from, string to, int maxTranslations, TranslateOptions options, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public GetTranslationsResponse EndGetTranslations(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public string Translate(string appId, string text, string from, string to)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginTranslate(string appId, string text, string from, string to, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public string EndTranslate(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void AddTranslationArray(string appId, Translation[] translations, string from, string to, TranslateOptions options)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginAddTranslationArray(string appId, Translation[] translations, string from, string to, TranslateOptions options, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndAddTranslationArray(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public GetTranslationsResponse[] GetTranslationsArray(string appId, string[] texts, string from, string to, int maxTranslations, TranslateOptions options)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginGetTranslationsArray(string appId, string[] texts, string from, string to, int maxTranslations, TranslateOptions options, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public GetTranslationsResponse[] EndGetTranslationsArray(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public string Speak(string appId, string text, string language, string format)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginSpeak(string appId, string text, string language, string format, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public string EndSpeak(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public TranslateArrayResponse[] TranslateArray(string appId, string[] texts, string from, string to, TranslateOptions options)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginTranslateArray(string appId, string[] texts, string from, string to, TranslateOptions options, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public TranslateArrayResponse[] EndTranslateArray(IAsyncResult result)
        {
            throw new NotImplementedException();
        }
    }
}