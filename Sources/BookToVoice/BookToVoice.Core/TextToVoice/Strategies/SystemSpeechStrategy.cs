using System;
using System.Collections.Generic;
using System.IO;

namespace BookToVoice.Core.TextToVoice.Strategies
{
    public class SystemSpeechStrategy : ITextToVoiceStrategy
    {
        private bool disposed = false;

        private static byte[] StartSpeak(string word)
        {
            var ms = new MemoryStream();
            using (System.Speech.Synthesis.SpeechSynthesizer synhesizer = new System.Speech.Synthesis.SpeechSynthesizer())
            {
                foreach (var voice in synhesizer.GetInstalledVoices())
                {
                    Console.WriteLine("select(y/n): " + voice.VoiceInfo.Name);
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Y)
                    {
                        synhesizer.SelectVoice(voice.VoiceInfo.Name);
                        synhesizer.SelectVoiceByHints(voice.VoiceInfo.Gender, voice.VoiceInfo.Age, 1, voice.VoiceInfo.Culture);
                        synhesizer.SetOutputToWaveStream(ms);
                        synhesizer.Speak(word);
                    }
                }
            }

            return ms.ToArray();
        }

        public void Execute(TextToVoiceModel model, string voiceName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetVoiceNames()
        {
            throw new NotImplementedException();
        }

        public int[] GetProgress()
        {
            throw new NotImplementedException();
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                   
                }
                // Note disposing has been done.
                disposed = true;
            }
        }
        #endregion IDisposable
    }
}