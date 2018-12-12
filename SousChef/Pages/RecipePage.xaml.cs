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
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
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
        #region Events

        public delegate void RecipeNameUpdatedEvent(Guid recipeGuid, string newName);
        public event RecipeNameUpdatedEvent RecipeNameUpdated;

        #endregion

        #region Properties

        public Guid recipeId { get; set; }
        public string recipeName { get; set; }

        #endregion

        public RecipePage()
        {
            this.InitializeComponent();

            backButton.Click += NavigateBack;
            forwardButton.Click += NavigateForward;
            refreshButton.Click += Refresh;
            splitPaneButton.Click += AddWebViewPane;

            recipeNameTextBox.ConfirmClicked += (sender, e) => RecipeNameUpdated?.Invoke(this.recipeId, recipeName);
        }


        #region Recipe page events

        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs args)
        {
            var recipeCache = new RecipeCache();

            // url
            recipeCache.Url = urlBar.Text;

            // Name
            recipeCache.Name = recipeName;

            // Take screenshot of recipeGrid
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(recipeGrid, (int)recipeGrid.ActualWidth, (int)recipeGrid.ActualHeight);

            recipeCache.CacheImage = renderTargetBitmap;

            // Get number of webview panels and each of their scroll values
            foreach (var pane in paneGrid.Children)
                recipeCache.RecipePanes.Add(((SCWebView)pane).GetCacheValues());

            // Save in custom cache
            RecipeCachingHelper.cache[recipeId] = recipeCache;

            // Navigate
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {            
            this.recipeId = (Guid)e.Parameter;

            // Check cache for that recipe id
            if (RecipeCachingHelper.cache.ContainsKey(recipeId))
            {
                var recipeCache = RecipeCachingHelper.cache[recipeId];

                // Restore the name
                recipeName = recipeCache.Name;

                // Restore URL
                urlBar.Text = recipeCache.Url;

                // Restore image and display that
                cacheImage.Source = recipeCache.CacheImage;
                cacheImage.Visibility = Visibility.Visible;

                // Tell panes to restore from cache
                foreach (var paneCache in recipeCache.RecipePanes)
                    RestorePaneFromCache(paneCache);
            }
            else
            {
                AddWebViewPane(null, null);
            }
        }

        #endregion

        #region Browser control

        private void Refresh(object sender, RoutedEventArgs e)
        {
            foreach (var scWebView in paneGrid.Children.OfType<SCWebView>())
                scWebView.Refresh();
        }

        private void NavigateForward(object sender, RoutedEventArgs e)
        {
            foreach (var scWebView in paneGrid.Children.OfType<SCWebView>())
                scWebView.NavigateForward();
        }

        private void NavigateBack(object sender, RoutedEventArgs e)
        {
            foreach (var scWebView in paneGrid.Children.OfType<SCWebView>())
                scWebView.NavigateBack();
        }

        private void NavigateAndUpdate(string url, Guid? except = null)
        {
            foreach (var scWebView in paneGrid.Children.OfType<SCWebView>().Where(x => !x.webViewId.Equals(except)))
                scWebView.Navigate(url);
        }

        private void UrlBar_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                NavigateAndUpdate(urlBar.Text);
        }

        #endregion

        #region Pane events

        private void AddPane(FrameworkElement pane)
        {
            paneGrid.ColumnDefinitions.Add(GridHelpers.GenerateGridColumn());
            paneGrid.Children.Add(pane);
            GridHelpers.SetElementCoordinates(pane, paneGrid.Children.Count() - 1, 0);

            // If we have 2 or more panes, give some padding to the keep them separated 
            if (paneGrid.Children.Count() >= 2)
            {
                pane.Margin = new Thickness(5, 0, 0, 0);
                var paneToTheLeft = paneGrid.Children[paneGrid.Children.Count() - 2];
                ((FrameworkElement)paneToTheLeft).Margin = new Thickness(((FrameworkElement)paneToTheLeft).Margin.Left, 0, 5, 0);
            }
        }

        private void AddWebViewPane(object sender, RoutedEventArgs e)
        {
            string initUrl = "https://www.google.com/";

            if (!string.IsNullOrEmpty(urlBar.Text))
                initUrl = urlBar.Text;

            var webView = new SCWebView(initUrl);

            webView.AddNavigationObserver(NotifiedOfNavigation);
            webView.AddPaneClosingObserver(ClosePaneWithGuid);

            AddPane(webView);
        }

        private void RestorePaneFromCache(PaneCache paneCache)
        {
            var element = PaneFactory.RestorePaneFromCache(paneCache);

            if (element.GetType() == typeof(SCWebView))
                ((SCWebView)element).AddNavigationObserver(NotifiedOfNavigation);

            ((IRecipePane)element).AddPaneClosingObserver(ClosePaneWithGuid);
            ((IRecipePane)element).AddPaneFinishedRestoringObserver(PaneRestoredFromCache);

            AddPane(element);
        }

        private void PaneRestoredFromCache(Guid paneId)
        {
            var paneCache = RecipeCachingHelper.cache[this.recipeId].RecipePanes.FirstOrDefault(x => x.Id.Equals(paneId));
            paneCache.Restored = true;

            if (RecipeCachingHelper.cache[this.recipeId].RecipePanes.All(x => x.Restored))
                cacheImage.Visibility = Visibility.Collapsed;
        }

        private void NotifiedOfNavigation(string url, Guid invokerGuid)
        {
            NavigateAndUpdate(url, invokerGuid);
            urlBar.Text = url;
        }

        private void ClosePaneWithGuid(Guid toRemove)
        {
            var webViewToRemove = paneGrid.Children.OfType<SCWebView>().FirstOrDefault(x => x.webViewId.Equals(toRemove));
            var columnIndexToRemove = paneGrid.Children.IndexOf(webViewToRemove);

            // Move all items with column index > columnIndexToRemove to the left
            var numberOfColumns = paneGrid.ColumnDefinitions.Count();
            for (int i = columnIndexToRemove + 1; i < numberOfColumns; i++)
                GridHelpers.SetElementCoordinates(paneGrid.Children[i] as SCWebView, i - 1, 0);

            // Remove the last column
            paneGrid.ColumnDefinitions.RemoveAt(numberOfColumns - 1);
            paneGrid.Children.Remove(webViewToRemove);
        }

        #endregion
    }
}
