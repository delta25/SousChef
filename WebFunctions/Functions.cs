using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

namespace WebFunctions
{
    [AllowForWeb]
    public sealed class ScrollValueRetrievalHelper
    {
        private int scrollValue;

        public int ScrollValue { get => scrollValue; set => scrollValue = value; }

        public void setScrollValue(double _scrollValue)
        {
            this.ScrollValue = (int)_scrollValue;
        }
    }
}
