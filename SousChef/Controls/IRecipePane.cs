using SousChef.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace SousChef.Controls
{
    public interface IRecipePane
    {
        PaneCache GetCacheValues();
        void RestoreFromCache(PaneCache paneCache);
    }
}
