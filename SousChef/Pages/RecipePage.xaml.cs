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

            backButton.Click += NavigateBack;
            forwardButton.Click += NavigateForward;
            refreshButton.Click += Refresh;
            splitPaneButton.Click += AddWebViewPane;

            AddWebViewPane(null, null);
        }

        public int NotifiedOfNavigation(string url, Guid invokerGuid)
        {
            NavigateAndUpdate(url, invokerGuid);
            urlBar.Text = url;
            return 1;
        }

        private void AddWebViewPane(object sender, RoutedEventArgs e)
        {
            string initUrl = "https://www.google.com/";

            webViewGrid.ColumnDefinitions.Add(GridHelpers.GenerateGridColumn());
            if (!string.IsNullOrEmpty(urlBar.Text))
                initUrl = urlBar.Text;

            var webView = new SCWebView(initUrl, NotifiedOfNavigation, ClosePaneWithGuid);
            webViewGrid.Children.Add(webView);
            GridHelpers.SetElementCoordinates(webView, webViewGrid.Children.Count() - 1, 0);

            // If we have 2 or more panels, give some padding to the right most panel
            if (webViewGrid.Children.Count() >= 2)
            {
                webView.Margin = new Thickness(5, 0, 0, 0);
                var panelToTheLeft = webViewGrid.Children[webViewGrid.Children.Count() - 2];
                ((SCWebView)panelToTheLeft).Margin = new Thickness(((SCWebView)panelToTheLeft).Margin.Left, 0, 5, 0);
            }
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            foreach (var scWebView in webViewGrid.Children.OfType<SCWebView>())
                scWebView.Refresh();
        }

        private void NavigateForward(object sender, RoutedEventArgs e)
        {
            foreach (var scWebView in webViewGrid.Children.OfType<SCWebView>())
                scWebView.NavigateForward();
        }

        private void NavigateBack(object sender, RoutedEventArgs e)
        {
            foreach (var scWebView in webViewGrid.Children.OfType<SCWebView>())
                scWebView.NavigateBack();
        }

        private void NavigateAndUpdate(string url, Guid? except = null)
        {
            foreach (var scWebView in webViewGrid.Children.OfType<SCWebView>().Where(x => !x.webViewId.Equals(except)))
                scWebView.Navigate(url);
        }

        private void UrlBar_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                NavigateAndUpdate(urlBar.Text);
        }

        private int ClosePaneWithGuid(Guid toRemove)
        {
            var webViewToRemove = webViewGrid.Children.OfType<SCWebView>().FirstOrDefault(x => x.webViewId.Equals(toRemove));
            webViewGrid.Children.Remove(webViewToRemove);
            return 1;
        }
    }
}
