using System;
using System.IO;
using System.Linq;
using BookToVoice.Core.TextToVoice;
using BookToVoice.Core.TextToVoice.Strategies;
using NUnit.Framework;

namespace BookToVoice.Core.Test.TextToVoice.Strategies
{

    [TestFixture]
    public class SpeechLibStrategyTest
    {
        private string testText =
@"Старт

Один. Два. Три.

Один. Два. Три.
       
конец";

    
        [Test]
        public void SpeechTest()
        {
            string filename = string.Format("test_{0:d_M_yyy_HH_mm_ss}.opus", DateTime.Now);
            SpeechLibStrategy strategy = SpeechLibStrategy.Create();
            var voices = strategy.GetVoiceNames().ToList();
            Assert.IsTrue(voices.Count() == 2, "voices: " + string.Join(", ", voices));

            var model = new TextToVoiceModel
                {
                    CurrentState = TextToVoiceModel.States.Run,
                    OutFilePath = filename,
                    FilePath = filename
                };

            model.SetText(testText);
            strategy.Execute(model, voices.Last());

            FileInfo fi = new FileInfo(filename);
            Assert.IsTrue(fi.Exists);
            Assert.IsTrue(fi.Length > 0);
            //fi.Delete();
        }
    }
}
