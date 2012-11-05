using System;

namespace ResEx.Core
{
    public interface IAutoTranslationContext
    {
        /// <summary>
        /// Raised before an item is to be automatically translated. Allows tampering with the source text.
        /// </summary>
        event EventHandler<AutoTranslationEventArgs<AutoTranslationItem>> BeforeItemAutoTranslation;

        /// <summary>
        /// Raised after an item is to be automatically translated. Allows tampering with the translation result.
        /// </summary>
        event EventHandler<AutoTranslationEventArgs<AutoTranslationResult>> AfterItemAutoTranslation;
    }
}