using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace SousChef.Controls
{
    public sealed class ConfirmCancelTextBox : Control
    {
        #region Dependency properties

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(ConfirmCancelTextBox), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ConfirmCancelTextBox), new PropertyMetadata(string.Empty, new PropertyChangedCallback(TextBoxValueChanged)));

        #endregion

        #region Properties

        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private Button saveTextChange;
        private Button cancelTextChange;
        private TextBox textBox;

        private string originalTextBoxValue;

        #endregion

        #region Events

        public delegate void ButtonClickedEvent(object sender, RoutedEventArgs e);
        public event ButtonClickedEvent ConfirmClicked;

        #endregion

        public ConfirmCancelTextBox()
        {
            this.DefaultStyleKey = typeof(ConfirmCancelTextBox);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BindUiVariables();
            AddEventListeners();
        }

        private void BindUiVariables()
        {
            saveTextChange = GetTemplateChild(nameof(saveTextChange)) as Button;
            cancelTextChange = GetTemplateChild(nameof(cancelTextChange)) as Button;
            textBox = GetTemplateChild(nameof(textBox)) as TextBox;
        }

        private void AddEventListeners()
        {
            this.saveTextChange.Click += SaveTextChangeClicked;
            this.cancelTextChange.Click += CancelTextChangeClicked;
            this.textBox.KeyUp += TextBoxKeyUp;
        }

        public void SetTextBoxValue(string value)
        {
            originalTextBoxValue = value;
        }

        private static void TextBoxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // If the value isn't the same as the original text, display the Confirm/Cancel buttons

        }

        private void TextBoxKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                SaveTextChangeClicked(null, null);
        }

        private void CancelTextChangeClicked(object sender, RoutedEventArgs e)
        {
            textBox.Text = originalTextBoxValue;
        }

        private void SaveTextChangeClicked(object sender, RoutedEventArgs e)
        {
            originalTextBoxValue = textBox.Text;
            ConfirmClicked?.Invoke(sender, e);
        }
    }
}
