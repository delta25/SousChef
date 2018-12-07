using SousChef.Controls;
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

namespace SousChef.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TimersListingPage : Page
    {
        public TimersListingPage()
        {
            this.InitializeComponent();            
            this.NavigationCacheMode = NavigationCacheMode.Required;

            var col1 = new ColumnDefinition();
            col1.Width = new GridLength(1, GridUnitType.Star);
            var col2 = new ColumnDefinition();
            col2.Width = new GridLength(1, GridUnitType.Star);

            myGrid.ColumnDefinitions.Add(col1);
            myGrid.ColumnDefinitions.Add(col2);


            webView = new WebView();
            webView.Navigate(new Uri("http://google.com"));

            myGrid.Children.Add(webView);
            Grid.SetColumn(webView, 0);

            var webView2 = new SCWebView(null, null,null);

            myGrid.Children.Add(webView2);
            Grid.SetColumn(webView2, 1);
            DispatcherTimerSetup();
        }

        WebView webView;

        DispatcherTimer dispatcherTimer;
        DateTimeOffset startTime;
        DateTimeOffset lastTime;
        DateTimeOffset stopTime;
        int timesTicked = 1;
        int timesToTick = 5;

        public void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            //IsEnabled defaults to false
            startTime = DateTimeOffset.Now;
            lastTime = startTime;

            dispatcherTimer.Start();
            //IsEnabled should now be true after calling start

        }

        void dispatcherTimer_Tick(object sender, object e)
        {
            DateTimeOffset time = DateTimeOffset.Now;
            TimeSpan span = time - lastTime;
            lastTime = time;
            //Time since last tick should be very very close to Interval

            timesTicked++;
            if (timesTicked > timesToTick)
            {
                stopTime = time;

                dispatcherTimer.Stop();
                //IsEnabled should now be false after calling stop

                span = stopTime - startTime;
                webView.Navigate(new Uri("http://bing.com"));
            }
        }
    }
}
