using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;

namespace Cutting
{
    /// <summary>
    /// Window wait for execution
    /// </summary>
    public partial class WindowBar : Window
    {
        private readonly List<int> timimings = new();
        private readonly MediaFile inputFile = new();
        private readonly string pathOutFile;
        private bool cancel = false;

        /// <summary>
        /// Start window for cut video
        /// </summary>
        /// <param name="timimings">video cutting time</param>
        /// <param name="inputFile">path to file</param>
        public WindowBar(List<int> timimings, string inputFile)
        {
            InitializeComponent();

            this.timimings = timimings;
            this.inputFile.Filename = inputFile;
            pathOutFile = inputFile.Substring(0, inputFile.LastIndexOf(@"\") + 1);

            Cut();
        }

        private string? CreateNameOutFile(int iTimings)
        {
            string pathInput = inputFile.Filename;
            int lastUderLining = pathInput.LastIndexOf('_');
            if (lastUderLining == -1)
                return null;
            string fSegmentPath = pathInput.Substring(pathOutFile.Length, lastUderLining - pathOutFile.Length);
            int sUnderLining = fSegmentPath.LastIndexOf('_');
            if (sUnderLining == -1)
                return null;
            fSegmentPath = pathInput.Substring(0, sUnderLining + 1);
            string sSegmentPath = pathInput.Substring(lastUderLining - 1, pathInput.Length - lastUderLining + 1);
            return $"{fSegmentPath}{timimings[iTimings]}{sSegmentPath}";
        }

        /// <summary>
        /// Function for cutting
        /// </summary>
        private async void Cut()
        {
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
                var options = new ConversionOptions();
                pbStatus.Maximum = timimings.Count;
                TextBar.Text = $"Создано 0 из {timimings.Count}";
                for (int i = 0; i < timimings.Count; i++)
                {
                    if (cancel)
                    {
                        Close();
                        return;
                    }

                    options.CutMedia(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(timimings[i]));

                    MediaFile nameOutFile = new MediaFile { Filename = pathOutFile + (CreateNameOutFile(i) ?? $"short_{timimings[i]}s.mp4") };

                    await Task.Run(() => engine.Convert(inputFile, nameOutFile, options));

                    TextBar.Text = $"Создано {i + 1} из {timimings.Count}";
                }
            }
            Close();
        }

        /// <summary>
        /// Button for cancel
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cancel = true;
            TextBar.Text = "Завершение";
        }
    }
}
