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

        private Func<string, Guid, int> notifyOfNavigation;
        private Func<Guid, int> notifyOfClosing;

        ScrollValueRetrievalHelper scrollValueRetrievalHelper = new ScrollValueRetrievalHelper();

        public string borderWidth { get => borderWidthInt + "px"; }
        public int borderWidthInt { get => 5; }

        private string currentUrl;

        public SCWebView()
        {
            this.InitializeComponent();

            borderLeft.PointerEntered += LeftBorderEntered;
            borderTop.PointerEntered += TopBorderEntered;
            borderRight.PointerEntered += RightBorderEntered;
            borderBottom.PointerEntered += BottomBorderEntered;
            //borderTop.DragEnter += TopBorderEntered;

            closeButton.Click += ClosePane;

            SetUpNavigationCheckTimer();
            SetUpScrollUpdateTimer();
            SetUpCloseCloseButtonBarTimer();
        }

        public SCWebView(string initUrl) : this()
        {
            webViewId = Guid.NewGuid();

            Navigate(initUrl);
        }

        #region Observer registrations

        public void AddNavigationObserver(Func<string, Guid, int> notifyOfNavigation)
        {
            this.notifyOfNavigation = notifyOfNavigation;
        }

        public void AddPaneClosingObserver(Func<Guid, int> closePaneWithGuid)
        {
            this.notifyOfClosing = closePaneWithGuid;
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
            closeButtonBar.Visibility = Visibility.Collapsed;
        }

        private void ShowCloseButtonBar()
        {
            if (closeCloseButtonBarTimer.IsEnabled) closeCloseButtonBarTimer.Stop();
            closeButtonBar.Visibility = Visibility.Visible;
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
            var displayedUrl = webView.Source.AbsoluteUri.ToString();
            if (currentUrl != displayedUrl)
            {
                webView.AddWebAllowedObject("ScrollHelper", scrollValueRetrievalHelper);

                currentUrl = displayedUrl;
                this.notifyOfNavigation(displayedUrl, this.webViewId);
            }
        }

        async void ScrollUpdateTimer_TickAsync(object sender, object e)
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

        private void WebView_NavigationStarting(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            currentUrl = args.Uri.AbsoluteUri;
            this.notifyOfNavigation(args.Uri.AbsoluteUri, this.webViewId);
        }

        private void WebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            currentUrl = args.Uri.AbsoluteUri;
            this.notifyOfNavigation(args.Uri.AbsoluteUri, this.webViewId);
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
            GetCacheValues();//webView.Refresh();
        }

        #endregion

        #region Cache methods

        public IPaneCache GetCacheValues()
        {
            Debug.WriteLine(scrollValueRetrievalHelper.ScrollValue);
            return new SCWebViewPaneCache
            {
                Id = this.webViewId,
                Url = this.currentUrl,
                ScrollValue = scrollValueRetrievalHelper.ScrollValue
            };
        }

        public void RestoreFromCache(IPaneCache paneCache)
        {
            SCWebViewPaneCache scWebViewPaneCache = (SCWebViewPaneCache)paneCache;
        }

        #endregion
    }

}
