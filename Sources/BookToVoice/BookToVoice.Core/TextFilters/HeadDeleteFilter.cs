using System.Text.RegularExpressions;

namespace BookToVoice.Core.TextFilters
{
    public class HeadDeleteFilter : ITextFilter
    {
        private readonly string _headEndLine;

        public HeadDeleteFilter(string headEndLine)
        {
            _headEndLine = headEndLine;
        }

        public string Execute(string inpute)
        {
            var t = Regex.Match(inpute, _headEndLine);
            if (t.Index > 0 && t.Index < inpute.Length / 2)
            {
                inpute = inpute.Remove(0, t.Index);
                inpute = inpute.Remove(0, inpute.IndexOf('\n'));
            }
            return inpute;
        }
    }
}
