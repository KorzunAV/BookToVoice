using System.Collections.Generic;
using System.Windows;
using BookToVoice.Core.TextToVoice;

namespace BookToVoice.Client
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Settings : Window
	{
		private IEnumerable<string> Voises
		{
			get
			{
				var manager = TextToVoiceManager.Create();
				return manager.GetVoices();
			}
		}

		public Settings()
		{
			InitializeComponent();
			cbVoiceName.ItemsSource = Voises;
		}

		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			BookToVoice.Core.Properties.Settings.Default.Save();
			this.Close();
		}
	}
}
