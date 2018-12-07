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

        public string borderWidth { get => borderWidthInt + "px"; }
        public int borderWidthInt { get => 5; }

        private string currentUrl;
        private bool closeButtonVisible;

        public SCWebView(string initUrl, Func<string, Guid, int> notifyOfNavigation, Func<Guid, int> closePaneWithGuid)
        {
            this.InitializeComponent();
            webViewId = Guid.NewGuid();

            this.notifyOfNavigation = notifyOfNavigation;
            this.notifyOfClosing = closePaneWithGuid;

            borderLeft.PointerEntered += LeftBorderEntered;
            borderTop.PointerEntered += TopBorderEntered;
            borderRight.PointerEntered += RightBorderEntered;
            borderBottom.PointerEntered += BottomBorderEntered;
                        
            //borderTop.DragEnter += TopBorderEntered;

            closeButton.Click += ClosePane;

            SetUpNavigationCheckTimer();
            SetUpCloseCloseButtonBarTimer();
            Navigate(initUrl);
        }        

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
