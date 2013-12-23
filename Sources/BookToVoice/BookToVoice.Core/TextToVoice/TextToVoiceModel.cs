using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;


namespace BookToVoice.Core.TextToVoice
{
    public class TextToVoiceModel : INotifyPropertyChanged, IComparable<TextToVoiceModel>
    {
        public enum States { Wait, Runing, Run, Done, Paused, Aborted }

        private int _currentLine;
        private string[] _textLines;
        private DateTime? _lastStart;
        private States _currentState;


        [XmlElement]
        public States CurrentState
        {
            get { return _currentState; }
            set
            {
                _currentState = value;
                if (_currentState == States.Run)
                {
                    _lastStart = DateTime.Now;
                }
                else if (_lastStart != null)
                {
                    WorkTime += (int)(DateTime.Now - _lastStart.Value).TotalSeconds;
                    _lastStart = null;
                }
                OnPropertyChanged("CurrentState");
            }
        }

        [XmlIgnore]
        public string StateInfo
        {
            get
            {
                if (CurrentState == States.Run)
                {
                    var curSec = (int)(WorkTime + (DateTime.Now - _lastStart.Value).TotalSeconds);
                    var expSec = (int)(TextLines.Length * curSec / CurrentLine);

                    return string.Format("{0} / {1} [{2} / {3}]", TextLines.Length, CurrentLine, SecondsToTimeFormate(expSec), SecondsToTimeFormate(curSec));
                }
                else if (WorkTime > 0)
                {
                    return string.Format("{0} [{1}]", CurrentState, SecondsToTimeFormate(WorkTime));
                }
                return CurrentState.ToString();
            }
        }

        [XmlElement]
        public string FilePath { get; set; }

        [XmlIgnore]
        public string NameWithoutExtension { get { return Path.GetFileNameWithoutExtension(FilePath) ?? string.Empty; } }

        [XmlIgnore]
        public string[] TextLines
        {
            get
            {
                return _textLines;
            }
        }

        [XmlElement]
        public string OutFilePath { get; set; }

        [XmlElement]
        public int CurrentLine
        {
            get { return _currentLine; }
            set
            {
                _currentLine = value;
                OnPropertyChanged("StateInfo");
            }
        }

        [XmlElement]
        public int WorkTime { get; set; }



        public TextToVoiceModel()
        {
            CurrentState = States.Wait;
        }

        public void SetText(string text)
        {
            _textLines = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private string SecondsToTimeFormate(double seconds)
        {
            return string.Format("{0}:{1}:{2}", (int)(seconds / 3600), (int)(seconds / 60) % 60, seconds % 60);
        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        #region IComparable

        public int CompareTo(TextToVoiceModel other)
        {
            return String.Compare(other.FilePath, FilePath, StringComparison.CurrentCultureIgnoreCase);
        }

        #endregion IComparable
    }
}
