using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using BookToVoice.Core.TextToVoice.Converters;
using NAudio.Wave;
using NLog;
using SpeechLib;

namespace BookToVoice.Core.TextToVoice.Strategies
{
    public class SpeechLibStrategy : ITextToVoiceStrategy
    {
        private readonly Logger _log;
        private bool _disposed;
        private SpVoice Voice { get; set; }
        public WaveFormat WaveFormat { get; set; }
        private double t1 = 0;
        private double t11 = 0;

        private SpeechLibStrategy()
        {
            _log = LogManager.GetCurrentClassLogger();
            Voice = new SpVoice
                {
                    Rate = Properties.Settings.Default.SpeedRate,
                };
            WaveFormat = new WaveFormat(Properties.Settings.Default.SampleRate, Properties.Settings.Default.Bits, Properties.Settings.Default.Channels);
            SetVoice(Properties.Settings.Default.VoiceName);
        }


        public static SpeechLibStrategy Create()
        {
            return new SpeechLibStrategy();
        }

        public void Execute(TextToVoiceModel model, string voiceName)
        {
            var time1 = DateTime.Now;
            //var spFileStream = new SpMemoryStream();
            //Voice.AudioOutputStream = spFileStream;

            using (var fs = new FileStream(model.OutFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            {
                //converter = new OpusConvertor();
                //using (var converter = new WaveToMp3Convertor(64, WaveFormat, fs))
                using (var converter = new OpusConvertor())
                {
                    converter.SetOutStream(fs);
                    while (model.CurrentLine < model.TextLines.Length && model.CurrentState == TextToVoiceModel.States.Run)
                    {
                        var spFileStream = new SpMemoryStream();
                        Voice.AudioOutputStream = spFileStream;
                        //  spFileStream.SetData(null);
                        var textLine = model.TextLines[model.CurrentLine];
                        var time11 = DateTime.Now;
                        var wavData = GetBytes(textLine, spFileStream);
                        var time21 = DateTime.Now;
                        t11 += (time21 - time11).TotalMilliseconds;
                        converter.ConvertAsyn(wavData);
                        model.CurrentLine++;
                    }
                    if (model.CurrentLine == model.TextLines.Length)
                    {
                        model.CurrentState = TextToVoiceModel.States.Done;
                    }
                }
            }

            var time2 = DateTime.Now;
            t1 += (time2 - time1).TotalMilliseconds;
        }

        public IEnumerable<string> GetVoiceNames()
        {
            foreach (ISpeechObjectToken token in Voice.GetVoices())
            {
                yield return token.GetDescription();
            }
        }

        private void SetVoice(string voiceName)
        {
            ISpeechObjectTokens tokens = Voice.GetVoices();
            for (int i = 0; i < tokens.Count; i++)
            {
                var item = tokens.Item(i);
                if (voiceName == item.GetDescription())
                {
                    Voice.Voice = item;
                }
            }
        }

        private byte[] GetBytes(string textLine, SpMemoryStream spFileStream)
        {
            bool isError = false;

            try
            {
                Voice.Speak(textLine, SpeechVoiceSpeakFlags.SVSFDefault);
                Voice.WaitUntilDone(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                isError = true;
            }

            return spFileStream.GetData() as byte[];
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    _log.Info(string.Format("t1={0}", t1));
                    _log.Info(string.Format("t11={0}", t11));
                }
                // Note disposing has been done.
                _disposed = true;
            }
        }
        #endregion IDisposable
    }
}