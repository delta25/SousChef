using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace SousChef.Controls
{
    public class GenericPane : Control
    {
        string borderWitdh = "5px";


        public static readonly DependencyProperty InnerContentProperty =
            DependencyProperty.Register("InnerContent", typeof(object), typeof(GenericPane), new PropertyMetadata(null));

        public object InnerContent
        {
            get { return (object)GetValue(InnerContentProperty); }
            set { SetValue(InnerContentProperty, value); }
        }

        public GenericPane()
        {
            this.DefaultStyleKey = typeof(GenericPane);
        }

    }
}
