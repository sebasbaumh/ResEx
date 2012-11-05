using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ResEx.Core
{
    public class ExclusionManager
    {
        public ExclusionManager()
        {
            this.ExclusionPatterns = new List<Exclusion>();
        }

        /// <summary>
        /// Gets or sets a list of regular expressions. Each match of these regular expressions that should be
        /// excluded from translation.
        /// </summary>
        public List<Exclusion> ExclusionPatterns { get; set; }

        private readonly IDictionary<string, string> placeholders = new Dictionary<string, string>();

        private const string PlaceholderPattern = "[### {0} ###]";

        private IEnumerable<string> GetActiveExclusionPatterns()
        {
            if (this.ExclusionPatterns == null || this.ExclusionPatterns.Count ==0)
            {
                return new string[] { };
            }

            return from p in this.ExclusionPatterns where p.Enabled select p.Pattern;
        }

        /// <summary>
        /// Replaces all matches of regular expressions found in <see cref="ExclusionPatterns"/> with placeholders
        /// that are not translatable by the translator.
        /// </summary>
        public string Exclude(string text)
        {
            if (this.ExclusionPatterns == null || this.ExclusionPatterns.Count == 0) return text;

            var captures = GetCaptures(text, this.GetActiveExclusionPatterns());
            
            foreach (Capture capture in captures)
            {
                text = text.Replace(capture.Value, this.GetPlaceholder(capture.Value));
            }

            return text;
        }

        /// <summary>
        /// Returns a list of captures of exclusions in the given text
        /// </summary>
        private static IEnumerable<Capture> GetCaptures(string text, IEnumerable<string> exclusionPatterns)
        {
            return from exclusionPattern in exclusionPatterns
                   select Regex.Matches(text, exclusionPattern)
                   into matches from Match match in matches from Group grp in match.Groups from Capture capture in grp.Captures select capture;
        }

        /// <summary>
        /// Gets a non translatable placeholder for the given text.
        /// </summary>
        private string GetPlaceholder(string text)
        {
            if (!this.placeholders.ContainsKey(text))
            {
                this.placeholders.Add(text, string.Format(PlaceholderPattern, this.placeholders.Count));
            }

            return this.placeholders[text];
        }

        /// <summary>
        /// Restores all placeholders created by <see cref="Exclude"/> with the original strings
        /// </summary>
        public string Restore(string text)
        {
            if (this.placeholders == null || this.placeholders.Count == 0) return text;

            var copyOfModifiedClosure = text;
            var placeHolders = this.placeholders.Where(placeholder => copyOfModifiedClosure.IndexOf(placeholder.Value, StringComparison.InvariantCultureIgnoreCase) != -1);
            foreach (var placeholder in placeHolders)
            {
                text = text.Replace(placeholder.Value, placeholder.Key);
            }

            return text;
        }

        /// <summary>
        /// Validates if the given translation includes all the items of the source item that should be excluded
        /// from translation. Returns a list of item that was supposed to be exluded by they didn't.
        /// Returns null if translation is ok.
        /// </summary>
        public string[] Validate(string translation, string source)
        {
            // get captures of excluded items in source
            var captures = GetCaptures(source, this.GetActiveExclusionPatterns());

            // get the captures that do not exist in translation
            var notFoundCaptures = from p in captures
                                   where translation.IndexOf(p.Value, StringComparison.InvariantCulture) == -1
                                   select p.Value;

            return notFoundCaptures.ToArray();
        }
    }
}