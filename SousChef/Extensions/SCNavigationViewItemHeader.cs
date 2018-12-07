using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using muxc = Microsoft.UI.Xaml.Controls;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace SousChef.Extensions
{
    public sealed class SCNavigationViewItemHeader : muxc.NavigationViewItemHeader
    {
        public SCNavigationViewItemHeader()
        {
            this.DefaultStyleKey = typeof(SCNavigationViewItemHeader);
        }

        private Button additionalActionButton;
        public event EventHandler<RoutedEventArgs> AdditionalActionButtonClicked;

        protected override void OnApplyTemplate()
        {
            additionalActionButton = GetTemplateChild(nameof(additionalActionButton)) as Button;
            additionalActionButton.Click += (s, e) => AdditionalActionButtonClicked?.Invoke(s, e);
        }
    }
}
