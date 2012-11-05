namespace ResEx.Core
{
    public class Exclusion
    {
        public Exclusion()
        {
            this.Enabled = true;
        }

        public Exclusion(string pattern)
        {
            this.Pattern = pattern;
            this.Enabled = true;
        }

        /// <summary>
        /// Gets or sets the regular expression pattern. All mathced items will be excluded from translation.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Gets or sets a string with sample(s) of string that will be excluded.
        /// Displayed to translator to understand what the pattern matches, in case the pattern is too complex
        /// of the translator is not aware of regular expressions.
        /// </summary>
        public string Sample { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the exclusion is enabled.
        /// </summary>
        public bool Enabled { get; set; }
    }
}