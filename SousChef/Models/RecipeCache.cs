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
        public List<IPaneCache> RecipePanes = new List<IPaneCache>();
        public RenderTargetBitmap CacheImage { get; set; }
    }

    public interface IPaneCache
    {

    }

    public class SCWebViewPaneCache : IPaneCache
    {
        public string Url { get; set; }
        public double ScrollValue { get; set; }
        public Guid Id { get; internal set; }
    }
}
