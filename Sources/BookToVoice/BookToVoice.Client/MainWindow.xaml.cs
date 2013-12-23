﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BookToVoice.Core.Extensions;
using BookToVoice.Core.TextFilters;
using BookToVoice.Core.TextToVoice;

namespace BookToVoice.Client
{
    public partial class MainWindow
    {
        private readonly TextToVoiceManager _factory = TextToVoiceManager.Create();
        private static readonly IEnumerable<string> Txt = new[] { "txt" };

        private static readonly Dictionary<string, string> CleanStrategy = new Dictionary<string, string>
            {
                {@"(\^\[\d+\] <#n_\d+>)", string.Empty},
                {"[/*<>_]", string.Empty},
                {"—", "-"},
                {"хитр", "хит р"},
            };

        private readonly ITextFilter[] _filters = new ITextFilter[]
            {
                new HeadDeleteFilter("(\\d+)(download)"),
                new EndNoteDeleteFilter("Примечания"),
                new SplitTextFilter(),
                new ReplaceByPatternsFilter(CleanStrategy)
            };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            var fbd = new System.Windows.Forms.FolderBrowserDialog
            {
                SelectedPath = string.IsNullOrEmpty(tbPath.Text) ? Properties.Settings.Default.DefaultDir : tbPath.Text
            };
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbPath.Text = fbd.SelectedPath;
            }
        }

        private void tbPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbPath.Text))
            {
                var directoryInfo = new DirectoryInfo(tbPath.Text);
                if (directoryInfo.Exists)
                {
                    lbFiles.Items.Clear();
                    foreach (
                        var fileInfo in
                            FileExtensions.EnumerateFindFiles(directoryInfo, Txt, SearchOption.AllDirectories))
                    {
                        lbFiles.Items.Add(fileInfo.FullName);
                    }
                    Properties.Settings.Default.DefaultDir = directoryInfo.FullName;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            foreach (var path in lbFiles.SelectedItems.Cast<string>())
            {
                if (!string.IsNullOrEmpty(path))
                {
                    ConvertFile(path);
                }
            }
        }

        /// <summary>
        /// Производит измениние файла в соответствии с набором правил.
        /// </summary>
        /// <param name="fileFullName">Полный путь к файлу</param>
        private void ConvertFile(string fileFullName)
        {
            Encoding enc = FileExtensions.GetEncodingType(fileFullName);
            if (enc != null)
            {
                var fileName = Path.GetFileName(fileFullName) ?? string.Empty;
                var filePath = Path.Combine(Properties.Settings.Default.PathToTextRep, fileName);

                var exist = new FileInfo(filePath).Exists;
                if (exist)
                {
                    exist = MessageBox.Show(string.Format("File: {0} already exist. owerride?", filePath), "File already exist.",
                                    MessageBoxButton.YesNo) == MessageBoxResult.No;
                }

                if (!exist)
                {
                    using (var sr = new StreamReader(fileFullName, enc))
                    {
                        using (var sw = new StreamWriter(filePath, false, Encoding.Default))
                        {
                            var text = sr.ReadToEnd();
                            foreach (var filter in _filters)
                            {
                                text = filter.Execute(text);
                            }
                            sw.Write(text);
                        }
                    }
                    _factory.Add(filePath);
                }
            }
        }

        private void btnToMp3_Click(object sender, RoutedEventArgs e)
        {
            _factory.StartAll();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            lbConvertedFiles.ItemsSource = _factory.GetData();
        }

        private void BtnStartPause_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var model = button.DataContext as TextToVoiceModel;
            if (model.CurrentState == TextToVoiceModel.States.Run)
            {
                _factory.Pause(model);
            }
            else if (model.CurrentState == TextToVoiceModel.States.Wait
                || model.CurrentState == TextToVoiceModel.States.Paused)
            {
                _factory.Start(model);
            }
        }

        private void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var model = button.DataContext as TextToVoiceModel;
            var fi = new FileInfo(model.FilePath);
            if (fi.Exists)
            {
                fi.Delete();
            }
            _factory.Remove(model);
        }

        private void MainWindow_OnClosed(object sender, EventArgs eventArgs)
        {
            _factory.Dispose();
        }

        private void btnClean_Click(object sender, RoutedEventArgs e)
        {
            lbFiles.Items.Clear();
        }

        private void MenuItemOptions_OnClick(object sender, RoutedEventArgs e)
        {
            var settings = new Settings();
            settings.Show();
        }

        private void BtnEdite_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var model = button.DataContext as TextToVoiceModel;

            System.Diagnostics.Process.Start(model.FilePath);
        }
    }
}
