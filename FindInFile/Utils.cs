using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FindInFile
{
    public static class ErrorLabelsController
    {
        private static Dictionary<string, Timer> timers = new Dictionary<string, Timer>();
        public static void ActiveErrorLabel(Label label, string errorMsg)
        {
            label.Content = errorMsg;
            label.Visibility = Visibility.Visible;
            StartErrorTimer(label);
        }

        public static void StartErrorTimer(Label label)
        {
            if (timers.TryGetValue(label.Name, out var labelTimer))
            {
                labelTimer?.Stop();
                labelTimer?.Dispose();
                timers.Remove(label.Name);
            }

            labelTimer = new Timer(5000);
            labelTimer.Elapsed += (sender, e) => DeactiveErrorLabel(sender, e, label);
            labelTimer.Enabled = true;
            timers.Add(label.Name, labelTimer);
        }

        public static void DeactiveErrorLabel(object sender, ElapsedEventArgs e, Label label)
        {
            if (sender != null)
                ((Timer)sender).Dispose();
            label.Dispatcher.Invoke(() => label.Visibility = Visibility.Hidden);
        }
    }

    public static class PlaceholderWorker
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.RegisterAttached("Placeholder", typeof(string), typeof(PlaceholderWorker), new PropertyMetadata(default(string)));
        public static void SetPlaceholder(UIElement element, string value) => element.SetValue(PlaceholderProperty, value);
        public static string GetPlaceholder(UIElement element) => (string)element.GetValue(PlaceholderProperty);

        public static readonly DependencyProperty PlaceholderColorProperty = DependencyProperty.RegisterAttached("PlaceholderColor", typeof(Brush), typeof(PlaceholderWorker), new PropertyMetadata(default(Brush)));
        public static void SetPlaceholderColor(UIElement element, Brush value) => element.SetValue(PlaceholderColorProperty, value);
        public static Brush GetPlaceholderColor(UIElement element) => (Brush)element.GetValue(PlaceholderColorProperty);

        public static readonly DependencyProperty IsPlaceholderActive = DependencyProperty.RegisterAttached("IsPlaceholderActive", typeof(bool), typeof(PlaceholderWorker), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty DefaultColor = DependencyProperty.RegisterAttached("DefaultColor", typeof(Brush), typeof(PlaceholderWorker), new PropertyMetadata(default(Brush)));

        public static void RemovePlaceholder(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if ((bool)textBox.GetValue(IsPlaceholderActive))
            {
                textBox.Text = "";
                textBox.Foreground = (Brush)textBox.GetValue(DefaultColor);
                textBox.SetValue(IsPlaceholderActive, false);
            }
        }

        public static void AddPlaceholder(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
                if (!(bool)textBox.GetValue(IsPlaceholderActive))
                {
                    textBox.Foreground = (Brush)textBox.GetValue(PlaceholderColorProperty);
                    textBox.Text = (string)textBox.GetValue(PlaceholderProperty);
                    textBox.SetValue(IsPlaceholderActive, true);
                }
        }

        public static void OnInit(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.SetValue(DefaultColor, textBox.Foreground);

            textBox.Foreground = (Brush)textBox.GetValue(PlaceholderColorProperty);
            textBox.Text = (string)textBox.GetValue(PlaceholderProperty);
            textBox.SetValue(IsPlaceholderActive, true);
        }

        public static bool GetPlaceholderActive(this TextBox textBox)
        {
            return (bool)textBox.GetValue(IsPlaceholderActive);
        }
    }
}
