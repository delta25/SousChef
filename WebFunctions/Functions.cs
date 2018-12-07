using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

namespace WebFunctions
{
    [AllowForWeb]
    public sealed class Functions
    {
        private int scrollValue;

        public void GetScrollValue(int _scrollValue)
        {
            scrollValue = 23535;
            //.WriteLine("Called from WebView! {0}", keyPress);
        }

        public bool Pepper()
        {
            return true;            //.WriteLine("Called from WebView! {0}", keyPress);
        }
    }
}
