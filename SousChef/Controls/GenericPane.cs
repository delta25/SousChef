using Windows.UI.Xaml.Controls;

namespace SousChef.Controls
{
    public class GenericPane : ContentControl
    {
        public string borderWidth
        {
            get { return "5px"; }
            set { borderWidth = value; }
        }

        public GenericPane()
        {
            this.DefaultStyleKey = typeof(GenericPane);
        }

    }
}
