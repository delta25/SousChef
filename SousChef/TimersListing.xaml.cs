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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SousChef
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TimersListing : Page
    {
        public TimersListing()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var col1 = new ColumnDefinition();
            col1.Width = new GridLength(1, GridUnitType.Star);
            var col2 = new ColumnDefinition();
            col2.Width = new GridLength(1, GridUnitType.Star);

            myGrid.ColumnDefinitions.Add(col1);
            myGrid.ColumnDefinitions.Add(col2);


            var webView = new WebView();
            webView.Navigate(new Uri("http://google.com"));

            myGrid.Children.Add(webView);
            Grid.SetColumn(webView, 0);

            var webView2 = new SousChef_WebView();            

            myGrid.Children.Add(webView2);
            Grid.SetColumn(webView2, 1);
        }
    }
}
