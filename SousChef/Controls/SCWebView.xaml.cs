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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SousChef.Controls
{
    public sealed partial class SCWebView : UserControl
    {
        private Func<string, Guid, int> notifyOfNavigation;

        public Guid webViewId { get; set; }

        private string currentUrl;

        public SCWebView(string initUrl, Func<string, Guid, int> notifyOfNavigation)
        {
            this.InitializeComponent();
            webViewId = Guid.NewGuid();

            this.notifyOfNavigation = notifyOfNavigation;

            closeButtonBar.PointerEntered += ShowCloseButtonBar;
            closeButtonBar.PointerExited += HideCloseButtonBar;
            closeButtonBar.PointerMoved += ShowCloseButtonBar;
            SetUpNavigationCheckTimer();
            Navigate(initUrl);
        }

        private void ShowCloseButtonBar(object sender, PointerRoutedEventArgs e)
        {
            closeButtonBar.Visibility = Visibility.Visible;
        }

        private void HideCloseButtonBar(object sender, PointerRoutedEventArgs e)
        {
            closeButtonBar.Visibility = Visibility.Collapsed;
        }

        private void SetUpNavigationCheckTimer()
        {
            DispatcherTimer navigationCheckTimer = new DispatcherTimer();
            navigationCheckTimer.Tick += NavigationCheckTimer_Tick;
            navigationCheckTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            navigationCheckTimer.Start();
        }

        public void Navigate(string url)
        {
            if (currentUrl == url) return;

            if (!(url.StartsWith("http://") || url.StartsWith("https://")))
                url = "http://" + url;

            webView.Navigate(new Uri(url));
        }

        void NavigationCheckTimer_Tick(object sender, object e)
        {
            var displayedUrl = webView.Source.AbsoluteUri.ToString();
            if (currentUrl != displayedUrl)
            {
                currentUrl = displayedUrl;
                this.notifyOfNavigation(displayedUrl, this.webViewId);
            }
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
            webView.Refresh();
        }
    }
}
