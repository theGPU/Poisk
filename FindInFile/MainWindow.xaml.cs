using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FindInFile
{
    public partial class MainWindow : Window
    {
        private static string selectedFilePath = "";
        public MainWindow()
        {
            InitializeComponent();

        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e) => PlaceholderWorker.RemovePlaceholder(sender, e);

        private void TextBox_LostFocus(object sender, RoutedEventArgs e) => PlaceholderWorker.AddPlaceholder(sender, e);

        private void TextBox_Initialized(object sender, EventArgs e) => PlaceholderWorker.OnInit(sender, e);

        private void OnSelectButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                SelectFileButton.Content = openFileDialog.SafeFileName;
            }
        }

        private void OnError(string errorText)
        {
            ErrorLabelsController.ActiveErrorLabel(ErrorLabel, errorText);
            ResultLabel.Content = "Ошибка";
        }

        private bool CheckErrors()
        {
            if (selectedFilePath is null || selectedFilePath == "")
            {
                OnError("Выберите файл");
                return false;
            }
            else if (SearchBox.Text is null || SearchBox.Text == (string)SearchBox.GetValue(PlaceholderWorker.PlaceholderProperty))
            {
                OnError("Введите текст для поиска");
                return false;
            }
            return true;
        }

        private void OnSearchButtonClick(object sender, RoutedEventArgs e)
        {
            if (CheckErrors())
            {
                string text = File.ReadAllText(selectedFilePath).ToLower();
                int count = Regex.Matches(text, SearchBox.Text).Count;
                ResultLabel.Content = $"Найдено совпадений в файле: {count}";
            }
        }
    }
}
