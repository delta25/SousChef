using Newtonsoft.Json;
using SousChef.Controls;
using SousChef.Helpers;
using SousChef.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
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

        public delegate void RecipeNameUpdatedHandler(Guid recipeGuid, string newName);
        public event RecipeNameUpdatedHandler RecipeNameUpdatedEvent;

        #endregion

        #region Properties

        public Guid recipeId { get; set; }
        public string recipeName { get; set; }

        private readonly string favouriteIconDisabled = "\uE734";
        private readonly string favouriteIconEnabled = "\uE735";

        private string favouriteRecipeIcon { get; set; }
        private bool recipeIsFavourite { get; set; }

        #endregion

        public RecipePage()
        {
            this.InitializeComponent();

            backButton.Click += NavigateBack;
            forwardButton.Click += NavigateForward;
            refreshButton.Click += Refresh;
            splitPaneButton.Click += ShowSplitPaneContextMenu;

            addWebViewButton.Click += AddWebViewPane;
            addTextViewButton.Click += AddTextViewPane;

            recipeNameTextBox.ConfirmClickedEvent += (sender, e) => RecipeNameUpdatedEvent?.Invoke(this.recipeId, recipeName);
        }

        #region Recipe page events

        private async Task<RecipeCache> GenerateRecipeCacheObject(bool isSaving = false)
        {
            var recipeCache = new RecipeCache();

            // url
            recipeCache.Url = urlBar.Text;

            // Recipe Id
            recipeCache.Id = this.recipeId;

            // Name
            recipeCache.Name = recipeName;

            // Favourite
            recipeCache.IsFavourite = recipeIsFavourite;

            if (!isSaving)
            {
                // Take screenshot of recipeGrid
                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
                await renderTargetBitmap.RenderAsync(recipeGrid, (int)recipeGrid.ActualWidth, (int)recipeGrid.ActualHeight);
                recipeCache.CacheImage = renderTargetBitmap;
            }

            // Get number of webview panels and each of their scroll values
            foreach (var pane in paneGrid.Children)
                recipeCache.RecipePanes.Add(((IRecipePane)pane).GetCacheValues());

            return recipeCache;
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs args)
        {
            var recipeCache = await GenerateRecipeCacheObject();

            // Save in custom cache
            RecipeCachingHelper.cache[recipeId] = recipeCache;

            // Check if we need to save by consulting the favourite icon
            SaveIfFavourite(recipeCache);

            Application.Current.Suspending -= OnAppSuspending;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Application.Current.Suspending += OnAppSuspending;

            this.recipeId = (Guid)e.Parameter;

            // Check cache for that recipe id
            if (RecipeCachingHelper.cache.ContainsKey(recipeId))
            {
                var recipeCache = RecipeCachingHelper.cache[recipeId];

                // Restore the name
                recipeName = recipeCache.Name;
                recipeNameTextBox.OriginalTextBoxValue = recipeName;

                // Is favourite
                recipeIsFavourite = recipeCache.IsFavourite;

                // Restore URL
                urlBar.Text = recipeCache.Url;

                // Restore image and display that
                if (recipeCache.CacheImage != null)
                {
                    cacheImage.Source = recipeCache.CacheImage;
                    cacheImage.Visibility = Visibility.Visible;
                }

                // Tell panes to restore from cache
                foreach (var paneCache in recipeCache.RecipePanes)
                    RestorePaneFromCache(paneCache);
            }
            else
            {
                AddWebViewPane(null, null);
            }
        }

        private void OnAppSuspending(object sender, SuspendingEventArgs e)
        {
            Application.Current.Suspending -= OnAppSuspending;
            SaveIfFavourite(null, null);
        }

        public void SaveIfFavourite(object sender, SuspendingEventArgs e)
        {
            SaveIfFavourite(GenerateRecipeCacheObject(isSaving: true).Result);
        }

        public async void SaveIfFavourite(RecipeCache recipeCache)
        {
            if (!recipeIsFavourite)
            {
                // Delete file if already there
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                IStorageItem recipeFile = await storageFolder.TryGetItemAsync($"{recipeCache.Id.ToString()}.rcp");
                if (recipeFile != null)
                    await recipeFile.DeleteAsync();
            }
            else
            {
                //Update fave
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile recipeFile = await storageFolder.CreateFileAsync($"{recipeCache.Id.ToString()}.rcp", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(recipeFile, JsonConvert.SerializeObject(recipeCache, settings));
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
            foreach (var scWebView in paneGrid.Children.OfType<SCWebView>().Where(x => !x.paneId.Equals(except)))
                scWebView.Navigate(url);
        }

        private void UrlBar_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                NavigateAndUpdate(urlBar.Text);
        }

        #endregion

        #region Pane events

        private void ShowSplitPaneContextMenu(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

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

            webView.NotifyOfNavigationEvent = NotifiedOfNavigation;
            webView.PaneClosingEvent = ClosePaneWithGuid;

            AddPane(webView);
        }

        private void AddTextViewPane(object sender, RoutedEventArgs e)
        {
            var textView = new SCTextView();

            textView.PaneClosingEvent = ClosePaneWithGuid;

            AddPane(textView);
        }

        private void RestorePaneFromCache(PaneCache paneCache)
        {
            var element = PaneFactory.RestorePaneFromCache(paneCache);

            if (element.GetType() == typeof(SCWebView))
                ((SCWebView)element).NotifyOfNavigationEvent = NotifiedOfNavigation;

            ((RecipePane)element).PaneClosingEvent = ClosePaneWithGuid;
            ((RecipePane)element).PaneFinishedRestoringEvent = PaneRestoredFromCache;

            ((RecipePane)element).RestoreFromCache(paneCache);

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
            var viewToRemove = paneGrid.Children.OfType<RecipePane>().FirstOrDefault(x => x.paneId.Equals(toRemove));
            var columnIndexToRemove = paneGrid.Children.IndexOf(viewToRemove);

            if (columnIndexToRemove == 0)
            {
                // Remove left padding for second item
                var element = (FrameworkElement)paneGrid.Children[columnIndexToRemove + 1];
                if (element != null)
                    element.Margin = new Thickness(0, 0, element.Margin.Right, 0);
            }

            if (columnIndexToRemove == paneGrid.Children.Count - 1)
            {
                // Remove right padding for second to last item
                var element = (FrameworkElement)paneGrid.Children[paneGrid.Children.Count - 1];
                if (element != null)
                    element.Margin = new Thickness(element.Margin.Left, 0, 0, 0);
            }

            // Move all items with column index > columnIndexToRemove to the left
            var numberOfColumns = paneGrid.ColumnDefinitions.Count();
            for (int i = columnIndexToRemove + 1; i < numberOfColumns; i++)
                GridHelpers.SetElementCoordinates(paneGrid.Children[i] as FrameworkElement, i - 1, 0);

            // Remove the last column
            paneGrid.ColumnDefinitions.RemoveAt(numberOfColumns - 1);
            paneGrid.Children.Remove(viewToRemove);
        }

        #endregion
    }
}
