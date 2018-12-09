using SousChef.Controls;
using SousChef.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SousChef.Helpers
{
    public static class PaneFactory
    {
        public static FrameworkElement RestorePaneFromCache(IPaneCache paneCache)
        {
            if (paneCache.GetType() == typeof(SCWebViewPaneCache))
            {
                var scWebViewPaneCache = (SCWebViewPaneCache)paneCache;
                var restoredScWebView = new SCWebView();
                restoredScWebView.RestoreFromCache(scWebViewPaneCache);
                return restoredScWebView;
            }

            return null;
        }
    }
}
