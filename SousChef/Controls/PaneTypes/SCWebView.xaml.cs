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
    public sealed partial class SCWebView : RecipePane
    {
        #region Properties 
        public delegate void NotifyOfNavigationHandler(string targetUrl, Guid initiatorGuid);
        public NotifyOfNavigationHandler NotifyOfNavigationEvent;

        ScrollValueRetrievalHelper scrollValueRetrievalHelper = new ScrollValueRetrievalHelper();

        private string currentUrl;

        private WebView webView;

        #endregion

        public SCWebView() :base()
        {
            this.InitializeComponent();

            webView = new WebView(WebViewExecutionMode.SeparateThread);
            genericPane.Content = webView;
            genericPane.PaneClosingEvent += (sender, args) => PaneClosingEvent(this.paneId);
        }

        public SCWebView(string initUrl) : this()
        {
            Navigate(initUrl);

            SetUpNavigationCheckTimer();
            SetUpScrollUpdateTimer();
        }
        
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
                this.NotifyOfNavigationEvent(displayedUrl, this.paneId);
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

        public override PaneCache GetCacheValues()
        {            
            return new SCWebViewPaneCache
            {
                Id = this.paneId,
                Url = this.currentUrl,
                ScrollValue = scrollValueRetrievalHelper.ScrollValue
            };
        }

        private SCWebViewPaneCache currentPaneCache;

        public override void RestoreFromCache(PaneCache paneCache)
        {
            currentPaneCache = (SCWebViewPaneCache)paneCache;

            paneId = currentPaneCache.Id;

            webView.DOMContentLoaded += RestoreWebViewFromCacheComplete;
            webView.Source = new Uri(currentPaneCache.Url);
        }
        
        private async void RestoreWebViewFromCacheComplete(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            currentUrl = args.Uri.AbsoluteUri;

             await webView.InvokeScriptAsync("eval",
                new[] { "function scroll(){window.scrollTo(0," + currentPaneCache.ScrollValue + ");} scroll();" });

            PaneFinishedRestoringEvent(this.paneId);
            SetUpNavigationCheckTimer();
            SetUpScrollUpdateTimer();
        }

        #endregion
    }

}
