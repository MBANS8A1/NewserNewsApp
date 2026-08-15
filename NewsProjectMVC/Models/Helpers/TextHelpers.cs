using NewsProjectMVC.Models.Db;

namespace NewsProjectMVC.Models.Helpers
{
    public static class TextHelpers
    {
        private const int WordsPerMinute = 240; // Average reading speed (I found this from online searching)

        /// <summary>
        /// Calculates the estimated reading time in minutes for a given text.
        /// </summary>
        /// <param name="text">The text (LongDescription) content to analyse.</param>
        /// <returns>The estimated reading time in minutes.</returns>
        public static int CalculateReadingTime(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return 0;
            }

            // Count the words, splitting by spaces and newlines
            var wordCount = text.Split([ ' ', '\r', '\n' ],
                                    StringSplitOptions.RemoveEmptyEntries).Length;

            if (wordCount == 0) return 0;

            // Calculate minutes and round up to the nearest whole number
            var minutes = Math.Ceiling((double)wordCount / WordsPerMinute);

            return Convert.ToInt32(minutes);
        }
    }
}
