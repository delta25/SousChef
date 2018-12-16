using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SousChef.Models;
using Windows.UI.Xaml.Controls;

namespace SousChef.Controls
{
    public abstract class RecipePane : UserControl, IRecipePane
    {
        public Guid paneId;

        public RecipePane()
        {
            this.paneId = Guid.NewGuid();
        }

        public delegate void PaneClosingHandler(Guid paneGuid);
        public PaneClosingHandler PaneClosingEvent;

        public delegate void PaneFinishedRestoringHandler(Guid paneGuid);
        public PaneFinishedRestoringHandler PaneFinishedRestoringEvent;

        public abstract PaneCache GetCacheValues();
        public abstract void RestoreFromCache(PaneCache paneCache);
    }
}
