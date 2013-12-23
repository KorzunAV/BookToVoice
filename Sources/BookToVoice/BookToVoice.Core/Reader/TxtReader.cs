using System.Collections.Generic;
using System.IO;

namespace BookToVoice.Core.Reader
{
    public class TxtReader : BaseReader
    {
        private readonly List<string> _extensions = new List<string> { ".txt" };
        public override List<string> Extensions
        {
            get { return _extensions; }
        }

        public override string Read(FileInfo fileInfo)
        {
            var sr = new StreamReader(fileInfo.FullName);
            return sr.ReadToEnd();
        }
    }
}
