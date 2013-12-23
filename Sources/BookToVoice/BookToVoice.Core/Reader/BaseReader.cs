using System.Collections.Generic;
using System.IO;

namespace BookToVoice.Core.Reader
{
    public class BaseReader
    {
        private readonly List<string> _extensions = new List<string>();
        public virtual List<string> Extensions
        {
            get { return _extensions; }
        }

        public virtual string Read(FileInfo fileInfo)
        {
            return string.Empty;
        }
    }
}
