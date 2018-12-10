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
        public delegate void ActionBarButtonClicked(object sender, EventArgs e);
        public event ActionBarButtonClicked CloseButtonClicked;

        public Button closeButton { get; set; }

        public PanelActionBar()
        {            
            this.DefaultStyleKey = typeof(PanelActionBar);
        }       

        protected override void OnApplyTemplate()
        {
            BindUIVariables();
            AddEventListeners();
        }

        private void BindUIVariables()
        {
            closeButton = GetTemplateChild(nameof(closeButton)) as Button;
        }

        private void AddEventListeners()
        {
            closeButton.Click += (sender, e) => CloseButtonClicked(sender, EventArgs.Empty);
        }
    }
}
