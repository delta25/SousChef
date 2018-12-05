using SousChef.Controls;
using SousChef.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SousChef.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RecipePage : Page
    {
        public RecipePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            webViewGrid.ColumnDefinitions.Add(GridHelpers.GenerateGridColumn());

            var defaultWebView = new SCWebView(urlBar);
            webViewGrid.Children.Add(defaultWebView);
            GridHelpers.SetElementCoordinates(defaultWebView, 0, 0);
        }

        private void UrlBar_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                foreach (var scWebView in webViewGrid.Children.OfType<SCWebView>())
                {
                    scWebView.Navigate(urlBar.Text);
                }
            }
        }
    }
}
