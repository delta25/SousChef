using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SousChef.Constants
{
    public class UnitConversions
    {
        public class Names
        {
            public const string Temperature = "Temperature";
            public const string CupsAndML = "Cups & ML";
            public const string CupsAndGrams = "Cups & Grams";
            public const string ButterAndGrams = "Butter Sticks & Grams";
        }

        public class Icons
        {
            private const string BasePath = "ms-appx:///Assets/";
            public const string Fahrenheit = BasePath + "fahrenheit.png";
            public const string Celsius = BasePath + "celsius.png";
            public const string Cup = BasePath + "cup.png";
            public const string ButterStick = BasePath + "butterStick.png";
            public const string Liquid = BasePath + "liquid.png";
            public const string Weight = BasePath + "weight.png";
        }
    }
}
