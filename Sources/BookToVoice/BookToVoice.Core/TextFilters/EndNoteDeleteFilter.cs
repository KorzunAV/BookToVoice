namespace BookToVoice.Core.TextFilters
{
    public class EndNoteDeleteFilter : ITextFilter
    {
        private readonly string _note;

        public EndNoteDeleteFilter(string note)
        {
            _note = note;
        }
        
        public string Execute(string inpute)
        {
            var id = inpute.LastIndexOf(_note, System.StringComparison.Ordinal);
            if (id > 0 && id > inpute.Length/2)
            {
                inpute = inpute.Remove(id);
            }
            return inpute;
        }
    }
}
