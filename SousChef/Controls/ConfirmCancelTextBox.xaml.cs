using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SousChef.Controls
{
    public sealed partial class ConfirmCancelTextBox : UserControl
    {
        public delegate void ButtonClicked(object sender, RoutedEventArgs e);
        public event ButtonClicked ConfirmClicked;

        private string originalValue;

        public string TextBoxPlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("TextBoxPlaceholderText", typeof(string), typeof(ConfirmCancelTextBox), new PropertyMetadata(string.Empty));


        public void SetValue(string value)
        {
            originalValue = value;
        }

        public ConfirmCancelTextBox()
        {
            this.InitializeComponent();

            this.saveTextChange.Click += SaveTextChangeClicked;
            this.cancelTextChange.Click += CancelTextChangeClicked;
            this.textBox.KeyUp += TextBoxKeyUp;
        }

        private void TextBoxKeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                SaveTextChangeClicked(null, null);
        }

        private void CancelTextChangeClicked(object sender, RoutedEventArgs e)
        {
            textBox.Text = originalValue;
        }

        private void SaveTextChangeClicked(object sender, RoutedEventArgs e)
        {
            originalValue = textBox.Text;
            ConfirmClicked(sender, e);
        }
    }
}
