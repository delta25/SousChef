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
        private Func<string, Guid, int> navigateFunc;

        public Guid webViewId { get; set; }

        private string currentUrl;

        public SCWebView(Func<string, Guid, int> navigateFunc)
        {
            this.InitializeComponent();

            webView.NavigationStarting += WebView_NavigationStarting;
            webView.NavigationCompleted += WebView_NavigationStarting;

            this.navigateFunc = navigateFunc;

            webViewId = Guid.NewGuid();

            Navigate("google.com");
        }

        public void Navigate(string url)
        {
            if (currentUrl == url) return;

            if (!(url.StartsWith("http://") || url.StartsWith("https://")))
                url = "http://" + url;

            webView.Navigate(new Uri(url));
        }

        private void WebView_NavigationStarting(WebView sender, WebViewNavigationCompletedEventArgs args)
        {

            currentUrl = args.Uri.AbsoluteUri;
            this.navigateFunc(args.Uri.AbsoluteUri, this.webViewId);
        }

        private void WebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            currentUrl = args.Uri.AbsoluteUri;
            this.navigateFunc(args.Uri.AbsoluteUri, this.webViewId);
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
