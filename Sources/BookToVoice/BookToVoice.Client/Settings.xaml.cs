using System.Collections.Generic;
using System.Windows;
using BookToVoice.Core.TextToVoice;

namespace BookToVoice.Client
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Settings
	{
		private IEnumerable<string> Voises
		{
			get
			{
				var manager = new TextToVoiceManager();
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
			Close();
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			Core.Properties.Settings.Default.Save();
			Close();
		}
	}
}
