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
        private Func<Guid, int> notifyOfClosing;

        public Guid webViewId { get; set; }

        public string borderWidth { get => "5px"; }

        private string currentUrl;
        private bool closeButtonVisible;

        public SCWebView(string initUrl, Func<string, Guid, int> notifyOfNavigation, Func<Guid, int> closePaneWithGuid)
        {
            this.InitializeComponent();
            webViewId = Guid.NewGuid();

            this.notifyOfNavigation = notifyOfNavigation;
            this.notifyOfClosing = closePaneWithGuid;

            borderLeft.PointerEntered += FlipCloseBluttonVisibility;
            borderTop.PointerEntered += FlipCloseBluttonVisibility;
            borderRight.PointerEntered += FlipCloseBluttonVisibility;
            borderBottom.PointerEntered += FlipCloseBluttonVisibility;

            borderTop.DragOver += FlipCloseBluttonVisibility;

            closeButton.Click += ClosePane;

            SetUpNavigationCheckTimer();
            Navigate(initUrl);
        }

        private void ClosePane(object sender, RoutedEventArgs e)
        {
            notifyOfClosing(this.webViewId);
        }

        private void FlipCloseBluttonVisibility(object sender, RoutedEventArgs e)
        {
            closeButtonVisible = !closeButtonVisible;
            closeButtonBar.Visibility = closeButtonVisible ? Visibility.Visible : Visibility.Collapsed;

            //Add timer to automatically hide the bar if it is currently showing after 3 seconds
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
