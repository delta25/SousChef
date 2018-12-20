using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SousChef.Models
{
    public class UnitConversion
    {
        public string Name { get; set; }
        public string LeftImage { get; set; }
        public string RightImage { get; set; }
        public List<IngredientConversion> IngredientConversions { get; set; }
    }

    public class IngredientConversion
    {
        public string Name { get; set; }
        public Func<double, double> FormulaLeftToRight { get; set; }
        public Func<double, double> FormulaRightToLeft { get; set; }
    }
}
