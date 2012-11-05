using System;
using ResEx.Core;
using ResEx.Core.PlugIns;

namespace ResEx.TranslationPlugin
{
    [PlugIn(Name = "Remove Trailing Space Plugin",
        Author = "Dimitris Papadimitriou",
        Description = "There is a problem with Microsoft translator and results have a space at the end, although original text does not. This plugin removes that obsolete character",
        Disabled = false)]
    public class RemoveTrailingSpacePlugin : IPlugIn
    {
        public void Initialize(IContext context)
        {
            context.AfterItemAutoTranslation += context_AfterItemAutoTranslation;
        }

        private static void context_AfterItemAutoTranslation(object sender, AutoTranslationEventArgs<AutoTranslationResult> e)
        {
            // there is a problem with Microsoft translator and results have a space at the end, although original text does not
            // following code removes that obsolete character
            var translationResult = e.Item;
            if (translationResult.Text.EndsWith(" ", StringComparison.InvariantCultureIgnoreCase) && !translationResult.OriginalText.EndsWith(" ", StringComparison.InvariantCultureIgnoreCase))
            {
                translationResult.Text = translationResult.Text.Substring(0, translationResult.Text.Length - 1);
            }
        }
    }
}