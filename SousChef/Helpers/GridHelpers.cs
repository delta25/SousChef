using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SousChef.Helpers
{
    public static class GridHelpers
    {
        public static ColumnDefinition GenerateGridColumn(int width = 1, GridUnitType unitType = GridUnitType.Star)
        => new ColumnDefinition() { Width = new GridLength(width, unitType) };

        public static void SetElementCoordinates(FrameworkElement element, int column, int row)
        {
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
        }

    }
}
