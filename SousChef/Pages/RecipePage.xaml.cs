using SousChef.Controls;
using SousChef.Helpers;
using SousChef.Models;
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
using Windows.UI.Xaml.Media.Imaging;
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

        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs args)
        {
            // Take screenshot of recipeGrid
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(recipeGrid, (int)recipeGrid.ActualWidth, (int)recipeGrid.ActualHeight);

            var recipeCache = new RecipeCache();

            // Get number of webview panels and each of their scroll values
            foreach( var panel in panelGrid.Children)
            {
                var panelCacheValue = ((SCWebView)panel).GetCacheValues();
                recipeCache.RecipePanels.Add(panelCacheValue);
            }
            // Get current URL
            // Save in custom cache
            // Navigate
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

            panelGrid.ColumnDefinitions.Add(GridHelpers.GenerateGridColumn());
            if (!string.IsNullOrEmpty(urlBar.Text))
                initUrl = urlBar.Text;

            var webView = new SCWebView(initUrl, NotifiedOfNavigation, ClosePaneWithGuid);
            panelGrid.Children.Add(webView);
            GridHelpers.SetElementCoordinates(webView, panelGrid.Children.Count() - 1, 0);

            // If we have 2 or more panels, give some padding to the keep them separated 
            if (panelGrid.Children.Count() >= 2)
            {
                webView.Margin = new Thickness(5, 0, 0, 0);
                var panelToTheLeft = panelGrid.Children[panelGrid.Children.Count() - 2];
                ((SCWebView)panelToTheLeft).Margin = new Thickness(((SCWebView)panelToTheLeft).Margin.Left, 0, 5, 0);
            }
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            foreach (var scWebView in panelGrid.Children.OfType<SCWebView>())
                scWebView.Refresh();
        }

        private void NavigateForward(object sender, RoutedEventArgs e)
        {
            foreach (var scWebView in panelGrid.Children.OfType<SCWebView>())
                scWebView.NavigateForward();
        }

        private void NavigateBack(object sender, RoutedEventArgs e)
        {
            foreach (var scWebView in panelGrid.Children.OfType<SCWebView>())
                scWebView.NavigateBack();
        }

        private void NavigateAndUpdate(string url, Guid? except = null)
        {
            foreach (var scWebView in panelGrid.Children.OfType<SCWebView>().Where(x => !x.webViewId.Equals(except)))
                scWebView.Navigate(url);
        }

        private void UrlBar_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                NavigateAndUpdate(urlBar.Text);
        }

        private int ClosePaneWithGuid(Guid toRemove)
        {
            var webViewToRemove = panelGrid.Children.OfType<SCWebView>().FirstOrDefault(x => x.webViewId.Equals(toRemove));
            var columnIndexToRemove = panelGrid.Children.IndexOf(webViewToRemove);

            // Move all items with column index > columnIndexToRemove to the left
            var numberOfColumns = panelGrid.ColumnDefinitions.Count();
            for (int i = columnIndexToRemove + 1; i < numberOfColumns; i++)            
                GridHelpers.SetElementCoordinates(panelGrid.Children[i] as SCWebView, i - 1, 0);
            
            // Remove the last column
            panelGrid.ColumnDefinitions.RemoveAt(numberOfColumns - 1);
            panelGrid.Children.Remove(webViewToRemove);

            GC.Collect();
            return 1;
        }
    }
}
