using System;
using System.Collections.Generic;
using System.Threading;
using BookToVoice.Core.TextToVoice.Converters;
using NLog;
using OpusWrapper.Opus.Presets;
using SpeechLib;

namespace BookToVoice.Core.TextToVoice.Strategies
{
    public class SpeechLibStrategy : ITextToVoiceStrategy
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly Options _options;
        private readonly ConvertorFactory.SupportedType _supportedType;
        private SpVoice Voice { get; set; }


        public SpeechLibStrategy(Options options, int speedRate, ConvertorFactory.SupportedType supportedType)
        {
            _options = options;
            _supportedType = supportedType;

            Voice = new SpVoice
                {
                    Rate = speedRate,
                };

            SetVoice(Properties.Settings.Default.VoiceName);
        }

        public void Execute(TextToVoiceModel model, string voiceName)
        {
            using (var converter = ConvertorFactory.GetConverter(model.OutFilePath, _options, _supportedType))
            {
                while (model.CurrentLine < model.TextLines.Length && model.CurrentState == TextToVoiceModel.States.Run)
                {
                    var textLine = model.TextLines[model.CurrentLine];
                    var wavData = GetBytes(textLine);
                    converter.ConvertAsyn(wavData);
                    model.CurrentLine++;
                }
                if (model.CurrentLine == model.TextLines.Length)
                {
                    model.CurrentState = TextToVoiceModel.States.Done;
                }
            }
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


        private bool TryGetBytes(string textLine, out byte[] data)
        {
            try
            {
                var spFileStream = new SpMemoryStream();
                Voice.AudioOutputStream = spFileStream;
                Voice.Speak(textLine);
                Voice.WaitUntilDone(Timeout.Infinite);
                data = spFileStream.GetData() as byte[];
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("{0}: \r\n {1}", ex, textLine));
            }
            data = null;
            return false;
        }

        private byte[] GetBytes(string textLine)
        {
            if (string.IsNullOrEmpty(textLine))
            {
                return new byte[0];
            }

            byte[] ret;
            if (TryGetBytes(textLine, out ret))
            {
                return ret;
            }

            var words = textLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length > 1)
            {
                var len = words.Length / 2;
                var fPart = GetBytes(string.Join(" ", words, 0, len));
                var lPart = GetBytes(string.Join(" ", words, len, words.Length - len));
                ret = new byte[fPart.Length + lPart.Length];
                Array.Copy(fPart, ret, fPart.Length);
                Array.Copy(lPart, ret, lPart.Length);
            }
            else
            {
                var len = textLine.Length / 2;
                var fPart = GetBytes(string.Join(" ", textLine, 0, len));
                var lPart = GetBytes(string.Join(" ", textLine, len, textLine.Length - len));
                ret = new byte[fPart.Length + lPart.Length];
                Array.Copy(fPart, ret, fPart.Length);
                Array.Copy(lPart, ret, lPart.Length);
            }
            return ret;
        }

        #region IDisposable
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                // Dispose managed resources.
                // Note disposing has been done.
                _disposed = true;
            }
        }
        #endregion IDisposable
    }
}