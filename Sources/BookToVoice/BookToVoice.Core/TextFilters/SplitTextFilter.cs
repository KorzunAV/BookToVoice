using System;
using System.Text.RegularExpressions;

namespace BookToVoice.Core.TextFilters
{
    /// <summary>
    /// Проводит разбивку текста по предложениям со сжатием по строкам.
    /// </summary>
    public class SplitTextFilter : ITextFilter
    {
        public string Execute(string inpute)
        {
            var sents = inpute.Split(new[] { ".", "?", "!"}, StringSplitOptions.RemoveEmptyEntries);

            for (int index = 0; index < sents.Length; index++)
            {
                var text = sents[index];
                bool isNewLine = text[0] == '\r' &&
                                 text[1] == '\n' &&
                                 (text[2] == ' ' || text[2] == '-' || text[2] == '\r');

                var rnbuf = Regex.Replace(text, @"\r\n", " ");
                var sbuf = Regex.Replace(rnbuf, @"[\s]{2,}", " ");
                sbuf = sbuf.Trim();

                sents[index] = (isNewLine ? "\r\n" : string.Empty) + sbuf;
            }
            return string.Join(". ", sents);
        }
    }
}
