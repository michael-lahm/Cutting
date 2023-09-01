using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;

namespace Cutting
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class WindowBar : Window
    {
        private readonly List<int> timimings = new();
        private readonly MediaFile inputFile = new();
        private readonly string outputFile;
        private bool cancel = false;

        public WindowBar(List<int> timimings, string inputFile)
        {
            InitializeComponent();

            this.timimings = timimings;
            this.inputFile.Filename = inputFile;
            outputFile = inputFile.Substring(0, inputFile.LastIndexOf(@"\") + 2);

            Cut();
        }

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
                        Close();

                    options.CutMedia(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(timimings[i]));

                    var nameOutFile = new MediaFile { Filename = outputFile + $@"short_{timimings[i]}.mp4" };
                    await Task.Run(() => engine.Convert(inputFile, nameOutFile, options));

                    TextBar.Text = $"Создано {i + 1} из {timimings.Count}";
                }
            }
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cancel = true;
        }
    }
}
