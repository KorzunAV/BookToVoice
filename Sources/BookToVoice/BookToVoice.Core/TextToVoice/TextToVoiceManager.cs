using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using BookToVoice.Core.Extensions;
using BookToVoice.Core.TextToVoice.Strategies;
using System.Linq;
using System;
using NLog;

namespace BookToVoice.Core.TextToVoice
{
    public class TextToVoiceManager : IDisposable
    {
        private static Thread _thread;
        public enum SupportedExtension { Mp3, Opus }
        private readonly Logger _log;
        private readonly ITextToVoiceStrategy _strategy;
        private readonly TextToVoiceModelConteiner _models;
        private readonly string _voiceName;
        private Timer _timer;
        private static readonly Object Synk = new object();

        private bool _disposed;

        private TextToVoiceManager()
        {
            _log = LogManager.GetCurrentClassLogger();
            _strategy = SpeechLibStrategy.Create();
            _voiceName = GetVoiceName();
            _models = new TextToVoiceModelConteiner();
        }

        public static TextToVoiceManager Create()
        {
            return new TextToVoiceManager();
        }

        public void Add(string fileFullNames)
        {
            var model = new TextToVoiceModel { FilePath = fileFullNames };
            model.OutFilePath = Path.Combine(Properties.Settings.Default.PathToVoiceRep, model.NameWithoutExtension);
            _models.Add(model);
        }

        public void AddRange(IEnumerable<string> fileFullNames)
        {
            foreach (var fileFullName in fileFullNames)
            {
                Add(fileFullName);
            }
        }

        public void Remove(TextToVoiceModel model)
        {
            model.CurrentState = TextToVoiceModel.States.Aborted;
            _models.Remove(model);
        }

        public void Remove(string fileFullNames)
        {
            var model = _models.FirstOrDefault(m => m.FilePath == fileFullNames);
            Remove(model);
        }

        public void Pause(TextToVoiceModel model)
        {
            if (model.CurrentState == TextToVoiceModel.States.Run && _thread.IsAlive)
            {
                model.CurrentState = TextToVoiceModel.States.Paused;
            }
        }

        public void Pause(string fileFullNames)
        {
            var model = _models.FirstOrDefault(m => m.FilePath == fileFullNames);
            Pause(model);
        }

        public void Start(TextToVoiceModel model)
        {
            if (model.CurrentState == TextToVoiceModel.States.Paused || model.CurrentState == TextToVoiceModel.States.Wait)
            {
                if (_thread != null && _thread.IsAlive)
                {
                    var rModel = _models.FirstOrDefault(m => m.CurrentState == TextToVoiceModel.States.Run);
                    if (rModel != null)
                    {
                        rModel.CurrentState = TextToVoiceModel.States.Paused;
                    }
                }
                model.CurrentState = TextToVoiceModel.States.Runing;
            }

            if (_timer == null)
            {
                _timer = new Timer(Execute, null, new TimeSpan(0, 0, 1), new TimeSpan(0, 0, 1));
            }
        }

        public void Start(string fileFullNames)
        {
            var model = _models.FirstOrDefault(m => m.FilePath == fileFullNames);
            Start(model);
        }

        public void StartAll()
        {
            foreach (var model in _models)
            {
                if (model.CurrentState == TextToVoiceModel.States.Paused)
                {
                    model.CurrentState = TextToVoiceModel.States.Wait;
                }
            }

            if (_timer == null)
            {
                _timer = new Timer(Execute, null, new TimeSpan(0, 0, 1), new TimeSpan(0, 0, 1));
            }
        }
        public TextToVoiceModelConteiner GetData()
        {
            return _models;
        }

        public void PauseAll()
        {
            foreach (var model in _models)
            {
                if (model.CurrentState == TextToVoiceModel.States.Run ||
                    model.CurrentState == TextToVoiceModel.States.Wait ||
                    model.CurrentState == TextToVoiceModel.States.Runing)
                {
                    model.CurrentState = TextToVoiceModel.States.Paused;
                }
            }
            _timer.Dispose();
            _timer = null;
        }

        private void Execute(object state)
        {
            lock (Synk)
            {
                if (_thread == null || !_thread.IsAlive)
                {
                    _thread = new Thread(ToMp3);

                    var model = _models.FirstOrDefault(m => m.CurrentState == TextToVoiceModel.States.Runing);
                    if (model == null)
                    {
                        model = _models.FirstOrDefault(m => m.CurrentState == TextToVoiceModel.States.Wait);
                    }
                    if (model != null)
                    {
                        _thread.Start(model);
                    }
                    else
                    {
                        _timer.Dispose();
                        _timer = null;
                    }
                }
            }
        }

        private void ToMp3(object model)
        {
            var typedModel = (TextToVoiceModel)model;
            Encoding enc = FileExtensions.GetEncodingType(typedModel.FilePath);
            if (enc != null)
            {
                using (var sr = new StreamReader(typedModel.FilePath, enc))
                {
                    var text = sr.ReadToEnd();
                    typedModel.SetText(text);
                    typedModel.CurrentState = TextToVoiceModel.States.Run;
                    _strategy.Execute(typedModel, _voiceName);
                }
            }
        }

        private string GetVoiceName()
        {
            var voiceName = Properties.Settings.Default.VoiceName;
            if (string.IsNullOrEmpty(voiceName))
            {
                voiceName = _strategy.GetVoiceNames().Last();
                Properties.Settings.Default.VoiceName = voiceName;
                Properties.Settings.Default.Save();
            }
            return voiceName;
        }

        public IEnumerable<string> GetVoices()
        {
            return _strategy.GetVoiceNames();
        }


        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    _log.Info("Dispose");
                    if (_timer != null)
                    {
                        _timer.Dispose();
                    }
                    if (_thread != null && _thread.IsAlive)
                    {
                        _log.Info("Dispose _thread");
                        _thread.Abort();
                    }
                    if (_strategy != null)
                    {
                        _log.Info("Dispose _strategy");
                        _strategy.Dispose();
                    }
                }
                // Note disposing has been done.
                _disposed = true;
            }
        }
        #endregion IDisposable
        
    }
}