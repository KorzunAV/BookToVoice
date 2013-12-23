using System.Collections.Generic;
using System.IO;

namespace BookToVoice.Core.Reader
{
    public class ReaderFactory
    {
        public List<BaseReader> Readers { get; set; }

        public string Read(FileInfo fileInfo)
        {
            string fileExtension = fileInfo.Extension;

            foreach(var reader in Readers)
            {
                if(reader.Extensions.Contains(fileExtension))
                {
                   return reader.Read(fileInfo); 
                }
            }
            return string.Empty;
        }

        public List<string> SupportedExtensions
        {
            get
            {
                List<string> extensions = new List<string>();
                foreach (var reader in Readers)
                {
                    extensions.AddRange( reader.Extensions);
                }
                return extensions;
            }
        }
    }
}
