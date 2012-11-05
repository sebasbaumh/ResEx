namespace ResEx.Core
{
    public class ResourceStringItem : ResourceItem
    {
        public const int UnlimitedLength = -1;

        public ResourceStringItem()
        {
            this.MaxLength = UnlimitedLength;
        }

        /// <summary>
        /// Gets or sets the maximum length (in characters) of the value
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Returns the number of words in this item
        /// </summary>
        public int CountWords()
        {
            if (!Common.StringTools.ValueNullOrEmpty(this.Value))
            {
                int counter = 1;    // starting from 1. even if no space is found then we have one word.
                bool previousCharWasSpace = false;
                foreach (var character in this.Value.ToString())
                {
                    if (character == ' ')
                    {
                        if (!previousCharWasSpace)
                        {
                            counter++;
                        }

                        previousCharWasSpace = true;
                    }
                    else
                    {
                        previousCharWasSpace = false; 
                    }
                }

                return counter;
            }

            return 0;
        }
    }
}