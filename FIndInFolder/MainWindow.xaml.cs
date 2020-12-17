using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace FindInFolder
{
    public partial class MainWindow : Window
    {
        private static string selectedFolderPath = "";
        private static ConcurrentBag<ResultRow> results;
        private static string searchText = "";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e) => PlaceholderWorker.RemovePlaceholder(sender, e);

        private void TextBox_LostFocus(object sender, RoutedEventArgs e) => PlaceholderWorker.AddPlaceholder(sender, e);

        private void TextBox_Initialized(object sender, EventArgs e) => PlaceholderWorker.OnInit(sender, e);

        private void OnSelectButtonClick(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog(this.GetIWin32Window()) == System.Windows.Forms.DialogResult.OK)
                {
                    selectedFolderPath = dialog.SelectedPath;
                    //SelectFolderButton.Content = dialog.
                }
            }
        }

        private void OnError(string errorText)
        {
            ErrorLabelsController.ActiveErrorLabel(ErrorLabel, errorText);
            //ResultLabel.Content = "Ошибка";
        }

        private bool CheckErrors()
        {
            if (selectedFolderPath is null || selectedFolderPath == "")
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

        private void SearchInFile(string filePath)
        {
            string text = File.ReadAllText(filePath).ToLower();
            int count = Regex.Matches(text, searchText).Count;
            if (count > 0)
            {
                //ResultView.Dispatcher.Invoke(() => ResultView.Items.Add(new ResultRow { Count = count, FileName = Path.GetFileName(filePath), FilePath = filePath }));
                results.Add(new ResultRow { Count = count, FileName = Path.GetFileName(filePath), FilePath = filePath });
            }
        }

        private void OnSearchButtonClick(object sender, RoutedEventArgs e)
        {
            ResultView.Items.Clear();
            results = new ConcurrentBag<ResultRow>();
            //ResultView.Dispatcher.Invoke(() => ResultView.Items.Add(new ResultRow { Count = 1, FileName = "test", FilePath = "test2" }));
            if (CheckErrors())
            {
                searchText = SearchBox.Text;
                string[] allfiles = Directory.GetFiles(selectedFolderPath, "*.*", SearchOption.AllDirectories);
                Parallel.ForEach(allfiles,
                    new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                    file => { SearchInFile(file); }
                );
            }
            foreach (var row in results)
            {
                ResultView.Items.Add(row);
            }
        }
    }
}
