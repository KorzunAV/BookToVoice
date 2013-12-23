using System.ComponentModel;

namespace BookToVoice.Client.Models
{
    public class ConvertInfoModel : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string State { get; set; }


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
    }
}
