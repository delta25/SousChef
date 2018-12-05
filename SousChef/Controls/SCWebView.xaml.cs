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
        private TextBox urlBar;

        public SCWebView(TextBox urlBar)
        {
            this.InitializeComponent();

            this.urlBar = urlBar;
            if (!string.IsNullOrEmpty(this.urlBar.Text))
                Navigate(this.urlBar.Text);
            else
                Navigate("http://www.google.com");

        }

        public void Navigate(string url)
        {
            if (!url.StartsWith("http://"))
                url = "http://" + url;

            webView.Navigate(new Uri(url));
        }
        

        private void WebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            urlBar.Text = args.Uri.AbsoluteUri;
        }
    }
}
