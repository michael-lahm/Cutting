using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using MediaToolkit.Model;
using MediaToolkit;

namespace Cutting
{
    /// <summary>
    /// Interaction logic for Main window
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<int> timings = new();
        private  string ?inputFile = null;
        private readonly int MAXDURVIDEO = 300;
        private readonly int MAXTIMEVIDEOCUT = 300;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Cheks and save choose file
        /// </summary>
        /// <param name="path">path to file</param>
        private void SaveChooseFile(string path)
        {
            var input = new MediaFile() { Filename = path };

            using (var engine = new Engine())
                engine.GetMetadata(input);

            if (input.Metadata.Duration.Seconds > MAXDURVIDEO)
            {
                MessageBox.Show($"Видео дольше {MAXDURVIDEO} секунд!");
                return;
            }
            else 
            {
                FileName.Text = path.Substring(path.LastIndexOf(@"\") + 1, path.Length - path.LastIndexOf(@"\") - 1);
                inputFile = path;
            }
        }

        /// <summary>
        /// Handler press open button
        /// </summary>
        private void ButtonOpen(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog( ) { Filter = "Файл MP4|*.mp4" };
            if (openFileDialog.ShowDialog() == true)
                SaveChooseFile(openFileDialog.FileName);
        }

        /// <summary>
        /// Handler drag fields
        /// </summary>
        private void InsertInButton(object sender, DragEventArgs e)
        {
            string path = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
            string extension = path.Substring(path.LastIndexOf("."), path.Length - path.LastIndexOf("."));
            if (extension == ".mp4")
                SaveChooseFile(path);
        }

        /// <summary>
        /// Handler press start cut button
        /// </summary>
        private void ButtonCut(object sender, RoutedEventArgs e)
        {
            if (inputFile == null)
            {
                MessageBox.Show("Файл не выбран!");
                return;
            }
            if (timings.Count == 0)
            {
                MessageBox.Show("Не выбрано время нарезки!");
                return;
            }
            if (timings.Max() > MAXTIMEVIDEOCUT)
            {
                MessageBox.Show($"Нарезка больше {MAXTIMEVIDEOCUT} сек");
                return;
            }

            new WindowBar(timings, inputFile).ShowDialog();
            MessageBox.Show("Нарезка завершена");
        }

        /// <summary>
        /// Handler input text
        /// </summary>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = ((TextBox)sender).Text;

            timings.Clear();
            int num = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                {
                    if (num != 0)
                    {
                        timings.Add(num);
                        num = 0;
                    }
                }
                else if (!char.IsDigit(text[i]))
                {
                    ((TextBox)sender).Text = text.Substring(0, (text.Length - 1));
                    TextBox1.SelectionStart = TextBox1.Text.Length;
                    TextBox1.SelectionLength = 0;
                    return;
                }
                else
                    num = num * 10 + (text[i] - 48);
            }
            if (num != 0)
                timings.Add(num);
        }
    }
}
