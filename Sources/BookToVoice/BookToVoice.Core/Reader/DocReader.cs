using System.Collections.Generic;
using System.IO;
using Microsoft.Office.Interop.Word;

namespace BookToVoice.Core.Reader
{
    public class DocReader : BaseReader
    {
        Application WordApp = new Application();
        Document _wordDoc;

        public DocReader()
        {
            WordApp.Visible = false;
            _wordDoc = new Document();
        }

        private readonly List<string> _extensions = new List<string> { ".doc", ".docx", ".rtf" };
        public override List<string> Extensions
        {
            get { return _extensions; }
        }
        
        public override string Read(FileInfo fileInfo)
        {
            object nullobj = System.Reflection.Missing.Value;
            object readOnly = true;
            object noEncodingDialog = true;
            object confirmConversions = false;
            object visible = false;
            object filePath = fileInfo.FullName;
            object openAndRepair = false;
            
            object openFormat = WdOpenFormat.wdOpenFormatAuto;

            _wordDoc = WordApp.Documents.Open(
                ref filePath, ref confirmConversions, ref readOnly, ref nullobj, ref nullobj,
                ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref openFormat, 
                ref nullobj, ref visible, ref openAndRepair, ref nullobj, ref noEncodingDialog,
                ref nullobj);
            
            // Make this document the active document.
            _wordDoc.Activate();


            string result = WordApp.ActiveDocument.Content.Text;

            _wordDoc.Close(ref nullobj, ref nullobj, ref nullobj);

            return result;
        }

    }
}