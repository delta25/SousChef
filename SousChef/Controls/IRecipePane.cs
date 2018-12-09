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
        void AddPaneClosingObserver(Func<Guid, int> closePaneWithGuid);

        IPaneCache GetCacheValues();
        void RestoreFromCache(IPaneCache paneCache);

    }
}
