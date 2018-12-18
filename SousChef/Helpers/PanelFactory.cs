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
        public static FrameworkElement RestorePaneFromCache(PaneCache paneCache)
        {
            if (paneCache.GetType() == typeof(SCWebViewPaneCache))
            {
                var restoredScWebView = new SCWebView();
                return restoredScWebView;
            }
            if (paneCache.GetType() == typeof(SCTextViewPaneCache))
            {
                var restoredScTextView = new SCTextView();
                return restoredScTextView;
            }

            return null;
        }
    }
}
