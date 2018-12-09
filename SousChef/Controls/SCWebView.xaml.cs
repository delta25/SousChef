using SousChef.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebFunctions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace SousChef.Controls
{
    public sealed partial class SCWebView : UserControl, IRecipePane
    {
        public Guid webViewId { get; set; }

        private Action<string, Guid> notifyOfNavigation;
        private Action<Guid> notifyOfClosing;
        private Action<Guid> finishedRestoringFromCache;

        ScrollValueRetrievalHelper scrollValueRetrievalHelper = new ScrollValueRetrievalHelper();

        public string borderWidth { get => borderWidthInt + "px"; }
        public int borderWidthInt { get => 5; }

        private string currentUrl;

        public SCWebView()
        {
            this.InitializeComponent();
            /*
            borderLeft.PointerEntered += LeftBorderEntered;
            borderTop.PointerEntered += TopBorderEntered;
            borderRight.PointerEntered += RightBorderEntered;
            borderBottom.PointerEntered += BottomBorderEntered;
            //borderTop.DragEnter += TopBorderEntered;

            closeButton.Click += ClosePane;*/
            SetUpCloseCloseButtonBarTimer();
        }

        public SCWebView(string initUrl) : this()
        {
            webViewId = Guid.NewGuid();

            Navigate(initUrl);

            SetUpNavigationCheckTimer();
            SetUpScrollUpdateTimer();
        }

        #region Observer registrations

        public void AddNavigationObserver(Action<string, Guid> notifyOfNavigation)
        {
            this.notifyOfNavigation = notifyOfNavigation;
        }

        public void AddPaneClosingObserver(Action<Guid> closePaneWithGuid)
        {
            this.notifyOfClosing = closePaneWithGuid;
        }

        public void AddPaneFinishedRestoringObserver(Action<Guid> paneRestoredFromCache)
        {
            this.finishedRestoringFromCache = paneRestoredFromCache;
        }

        #endregion

        #region Border Events

        private void LeftBorderEntered(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(sender as UIElement);
            //Coming in from the left
            if (p.Position.X < (borderWidthInt - 1) * .5)
                ShowCloseButtonBar();
            else
                HideCloseButtonBar();
        }

        private void TopBorderEntered(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(sender as UIElement);
            //Coming in from the left
            if (p.Position.Y < (borderWidthInt - 1) * .5)
                ShowCloseButtonBar();
            else
                HideCloseButtonBar();
        }

        private void RightBorderEntered(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(sender as UIElement);
            //Coming in from the left
            if (p.Position.X < (borderWidthInt - 1) * .5)
                HideCloseButtonBar();
            else
                ShowCloseButtonBar();
        }

        private void BottomBorderEntered(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(sender as UIElement);
            //Coming in from the left
            if (p.Position.Y < (borderWidthInt - 1) * .5)
                HideCloseButtonBar();
            else
                ShowCloseButtonBar();
        }

        #endregion

        #region Close button bar

        DispatcherTimer closeCloseButtonBarTimer;

        private void SetUpCloseCloseButtonBarTimer()
        {
            closeCloseButtonBarTimer = new DispatcherTimer();
            closeCloseButtonBarTimer.Tick += HideCloseButtonBar;
            closeCloseButtonBarTimer.Interval = new TimeSpan(0, 0, 3);
        }

        private void HideCloseButtonBar(object sender = null, object e = null)
        {
            if (closeCloseButtonBarTimer.IsEnabled) closeCloseButtonBarTimer.Stop();
            //closeButtonBar.Visibility = Visibility.Collapsed;
        }

        private void ShowCloseButtonBar()
        {
            if (closeCloseButtonBarTimer.IsEnabled) closeCloseButtonBarTimer.Stop();
           // closeButtonBar.Visibility = Visibility.Visible;
            closeCloseButtonBarTimer.Start();
        }


        private void ClosePane(object sender, RoutedEventArgs e)
        {
            notifyOfClosing(this.webViewId);
        }

        #endregion

        #region Timers

        private void SetUpNavigationCheckTimer()
        {
            DispatcherTimer navigationCheckTimer = new DispatcherTimer();
            navigationCheckTimer.Tick += NavigationCheckTimer_Tick;
            navigationCheckTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            navigationCheckTimer.Start();
        }

        private void SetUpScrollUpdateTimer()
        {
            DispatcherTimer scrollUpdateTimer = new DispatcherTimer();
            scrollUpdateTimer.Tick += ScrollUpdateTimer_TickAsync;
            scrollUpdateTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            scrollUpdateTimer.Start();
        }

        void NavigationCheckTimer_Tick(object sender, object e)
        {
            if (webView.Source == null) return;

            var displayedUrl = webView.Source.AbsoluteUri.ToString();
            if (currentUrl != displayedUrl)
            {
                webView.AddWebAllowedObject("ScrollHelper", scrollValueRetrievalHelper);

                currentUrl = displayedUrl;
                this.notifyOfNavigation(displayedUrl, this.webViewId);
            }
        }

        private async void ScrollUpdateTimer_TickAsync(object sender, object e)
        {
            try
            {
                await webView.InvokeScriptAsync("eval",
                    new[] {
                "ScrollHelper.setScrollValue(window.pageYOffset || document.documentElement.scrollTop)"
                    });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion

        #region Browser functions

        public void Navigate(string url)
        {
            if (currentUrl == url) return;

            if (!(url.StartsWith("http://") || url.StartsWith("https://")))
                url = "http://" + url;

            webView.AddWebAllowedObject("ScrollHelper", scrollValueRetrievalHelper);

            webView.Navigate(new Uri(url));
        }


        internal void NavigateBack()
        {
            webView.GoBack();
        }

        internal void NavigateForward()
        {
            webView.GoForward();
        }

        internal void Refresh()
        {
            webView.Refresh();
        }

        #endregion

        #region Cache methods

        public PaneCache GetCacheValues()
        {
            Debug.WriteLine(scrollValueRetrievalHelper.ScrollValue);
            return new SCWebViewPaneCache
            {
                Id = this.webViewId,
                Url = this.currentUrl,
                ScrollValue = scrollValueRetrievalHelper.ScrollValue
            };
        }

        private SCWebViewPaneCache currentPaneCache;

        public void RestoreFromCache(PaneCache paneCache)
        {
            currentPaneCache = (SCWebViewPaneCache)paneCache;

            webViewId = currentPaneCache.Id;

            webView.DOMContentLoaded += RestoreWebViewFromCacheComplete;
            webView.Source = new Uri(currentPaneCache.Url);
        }
        
        private async void RestoreWebViewFromCacheComplete(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            currentUrl = args.Uri.AbsoluteUri;

             await webView.InvokeScriptAsync("eval",
                new[] { "function scroll(){window.scrollTo(0," + currentPaneCache.ScrollValue + ");} scroll();" });

            finishedRestoringFromCache(this.webViewId);
            SetUpNavigationCheckTimer();
            SetUpScrollUpdateTimer();
        }

        #endregion
    }

}
