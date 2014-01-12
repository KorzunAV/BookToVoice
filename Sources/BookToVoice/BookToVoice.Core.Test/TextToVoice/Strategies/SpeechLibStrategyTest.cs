using System;
using System.IO;
using System.Linq;
using BookToVoice.Core.TextToVoice;
using BookToVoice.Core.TextToVoice.Converters;
using BookToVoice.Core.TextToVoice.Strategies;
using NUnit.Framework;
using OpusWrapper.Opus.Presets;

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
            var testCase = new[] { 6, 10, 12, 19, 24, 32, 64, 128, 256, 510 };
            var dir = string.Format("test_{0:d_M_yyy_HH_mm_ss}", DateTime.Now);
            Directory.CreateDirectory(dir);
            for (int i = 0; i < testCase.Length; i++)
            {
                string filename = string.Format("{0}\\{1}", dir, testCase[i]);
                var opt = new Options
                    {
                        BitRate = BitRate.Create(testCase[i])
                    };
                Convert(filename, opt, ConvertorFactory.SupportedType.Opus);
                var fi = new FileInfo(filename + ".opus");
                Assert.IsTrue(fi.Exists);
                Assert.IsTrue(fi.Length > 0);
            }
            string filename2 = string.Format("{0}\\{1}", dir, 1);
            Convert(filename2, new Options(), ConvertorFactory.SupportedType.Mp3);
            var fi2 = new FileInfo(filename2 + ".mp3");
            Assert.IsTrue(fi2.Exists);
            Assert.IsTrue(fi2.Length > 0);
        }

        private void Convert(string filename, Options options, ConvertorFactory.SupportedType supportedType)
        {
            var strategy = new SpeechLibStrategy(options, 4, supportedType);
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
        }
    }
}
