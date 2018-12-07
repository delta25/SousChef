using SousChef.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SousChef.Helpers
{
    public static class RecipeCachingHelper
    {
        public static Dictionary<Guid, RecipeCache> cache = new Dictionary<Guid, RecipeCache>();
    }
}
