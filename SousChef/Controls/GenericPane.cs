using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace SousChef.Controls
{
    public class GenericPane : ContentControl
    {
        /// <summary>
        /// Here only for some testing. Will be remmoved later
        /// </summary>
        public string borderWidth
        {
            get { return "5px"; }
            set { borderWidth = value; }
        }

        public int borderWidthInt = 0;

        private Grid borderLeft;
        private Grid borderTop;
        private Grid borderRight;
        private Grid borderBottom;
        public PanelActionBar panelActionBar;
        
        public delegate void PaneClosingHandler(object sender, EventArgs e);
        public event PaneClosingHandler PaneClosingEvent;

        public GenericPane()
        {
            this.DefaultStyleKey = typeof(GenericPane);
        }

        protected override void OnApplyTemplate()
        {
            borderWidthInt = int.Parse((string)App.Current.Resources["GenericPane.BorderWidth"]);

            BindUiVariables();
            AddEventListeners();

            SetUpCloseCloseButtonBarTimer();
        }

        private void BindUiVariables()
        {

            borderLeft = GetTemplateChild(nameof(borderLeft)) as Grid;
            borderTop = GetTemplateChild(nameof(borderTop)) as Grid;
            borderRight = GetTemplateChild(nameof(borderRight)) as Grid;
            borderBottom = GetTemplateChild(nameof(borderBottom)) as Grid;
            panelActionBar = GetTemplateChild(nameof(panelActionBar)) as PanelActionBar;
        }

        private void AddEventListeners()
        {
            borderLeft.PointerEntered += LeftBorderEntered;
            borderTop.PointerEntered += TopBorderEntered;
            borderRight.PointerEntered += RightBorderEntered;
            borderBottom.PointerEntered += BottomBorderEntered;
            //borderTop.DragEnter += TopBorderEntered;

            panelActionBar.CloseButtonClickedEvent += ClosePane;
        }

        #region Border Events

        private void LeftBorderEntered(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(sender as UIElement);
            //Coming in from the left
            if (p.Position.X < (borderWidthInt - 1) * .5)
                ShowCloseButtonBar();
            else
                HideCloseButtonBar();
        }

        private void TopBorderEntered(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(sender as UIElement);
            //Coming in from the left
            if (p.Position.Y < (borderWidthInt - 1) * .5)
                ShowCloseButtonBar();
            else
                HideCloseButtonBar();
        }

        private void RightBorderEntered(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(sender as UIElement);
            //Coming in from the left
            if (p.Position.X < (borderWidthInt - 1) * .5)
                HideCloseButtonBar();
            else
                ShowCloseButtonBar();
        }

        private void BottomBorderEntered(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(sender as UIElement);
            //Coming in from the left
            if (p.Position.Y < (borderWidthInt - 1) * .5)
                HideCloseButtonBar();
            else
                ShowCloseButtonBar();
        }

        #endregion


        #region Close button bar

        DispatcherTimer closeCloseButtonBarTimer;

        private void SetUpCloseCloseButtonBarTimer()
        {
            closeCloseButtonBarTimer = new DispatcherTimer();
            closeCloseButtonBarTimer.Tick += HideCloseButtonBar;
            closeCloseButtonBarTimer.Interval = new TimeSpan(0, 0, 3);
        }

        private void HideCloseButtonBar(object sender = null, object e = null)
        {
            if (closeCloseButtonBarTimer.IsEnabled) closeCloseButtonBarTimer.Stop();
            panelActionBar.Visibility = Visibility.Collapsed;
        }

        private void ShowCloseButtonBar()
        {
            if (closeCloseButtonBarTimer.IsEnabled) closeCloseButtonBarTimer.Stop();
            panelActionBar.Visibility = Visibility.Visible;
            closeCloseButtonBarTimer.Start();
        }

        private void ClosePane(object sender, EventArgs e)
        {
            PaneClosingEvent(sender, EventArgs.Empty);
        }

        #endregion

    }
}
