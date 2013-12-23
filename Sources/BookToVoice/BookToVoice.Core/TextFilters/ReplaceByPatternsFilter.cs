using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BookToVoice.Core.TextFilters
{
    /// <summary>
    /// Производит замену текста на основе набора регулярных выражений
    /// </summary>
    public class ReplaceByPatternsFilter : ITextFilter
    {
        private readonly Dictionary<string, string> _replacePatterns;

        public ReplaceByPatternsFilter(Dictionary<string, string> replacePatterns)
        {
            _replacePatterns = replacePatterns;
        }

        public string Execute(string inpute)
        {
            foreach (var pattern in _replacePatterns)
            {
                inpute = Regex.Replace(inpute, pattern.Key, pattern.Value);
            }
            return inpute;
        }
    }
}
