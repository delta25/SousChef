using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace SousChef.Controls
{    
    public sealed class PanelActionBar : Control
    {
        public PanelActionBar()
        {            
            this.DefaultStyleKey = typeof(PanelActionBar);
        }       

        protected override void OnApplyTemplate()
        {
        }
    }
}
