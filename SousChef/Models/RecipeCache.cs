using Newtonsoft.Json;
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
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public List<PaneCache> RecipePanes = new List<PaneCache>();
        public bool IsFavourite { get; set; }

        [JsonIgnore]
        public RenderTargetBitmap CacheImage { get; set; }
    }

    public class PaneCache
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public bool Restored { get; set; }
    }

    public class SCWebViewPaneCache : PaneCache
    {
        public string Url { get; set; }

        [JsonIgnore]
        public double ScrollValue { get; set; }
    }
}
