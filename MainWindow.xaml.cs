using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Cutting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<int> timings = new();
        private  string ?inputFile = null;
        private readonly int MaxTimeVideoCut = 300;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonOpen(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog( ) { Filter = "Файл MP4|*.mp4" };
            if (openFileDialog.ShowDialog() == true)
                inputFile = openFileDialog.FileName;
        }

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

            new WindowBar(timings, inputFile).ShowDialog();
            MessageBox.Show("Нарезка завершена");
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = ((TextBox)sender).Text;

            timings.Clear();
            int num = 0;
            for (int i = 0; i < text.Length; i++) //------------------------------переработать обработку ввода
            {
                if (!char.IsDigit(text[i]))
                {
                    ((TextBox)sender).Text = text.Substring(0, (text.Length - 1));
                    TextBox1.SelectionStart = TextBox1.Text.Length;
                    TextBox1.SelectionLength = 0;
                    return;
                }

                if (text[i] == ' ' || i + 1 == text.Length)
                {
                    if (num == 0)
                        MessageBox.Show("Нарезка не может быть 0 сек");
                    else if (num > MaxTimeVideoCut)
                        MessageBox.Show($"Нарезка больше {MaxTimeVideoCut} сек");
                    else
                    {
                        num = num * 10 + (text[i] - 48);
                        timings.Add(num);
                        num = 0;
                    }
                }
            }
        }
    }
}
