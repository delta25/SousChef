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

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace SousChef.Controls
{
    [ContentProperty(Name = "Content")]
    public sealed class TestingControl : Control
    {
        public TestingControl()
        {
            Resources.Add(new KeyValuePair<object, object>("myHeight", 200));
            this.DefaultStyleKey = typeof(TestingControl);
        }

        public static readonly string myText = "test"; 

        protected override void OnApplyTemplate()
        {
            var x = App.Current.Resources["myHeight2"];
            var y = (int)Resources["myHeight"];
        }

        public object Content
        {
            get { return (string)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(string), typeof(TestingControl), new PropertyMetadata(string.Empty));
    }
}
