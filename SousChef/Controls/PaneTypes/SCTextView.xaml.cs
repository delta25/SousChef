using SousChef.Models;
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
    public sealed partial class SCTextView : RecipePane
    {
        public SCTextView()
        {
            this.InitializeComponent();

            genericPane.PaneClosingEvent += (sender, args) => PaneClosingEvent(this.paneId);
        }

        public override PaneCache GetCacheValues()
        {
            return null;
        }

        public override void RestoreFromCache(PaneCache paneCache)
        {
            
        }

    }
}
