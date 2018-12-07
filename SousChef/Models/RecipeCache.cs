using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace SousChef.Models
{
    public class RecipeCache
    {
        public string Url { get; set; }
        public List<IPanelCache> RecipePanels = new List<IPanelCache>();
        public RenderTargetBitmap CacheImage { get; set; }
    }

    public interface IPanelCache
    {

    }

    public class SCWebViewPanelCache : IPanelCache
    {
        public double ScrollValue { get; set; }
    }
}
