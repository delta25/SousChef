using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SousChef.Constants
{
    public class IngredientConversions
    {
        public class Names
        {
            public const string FahrenheitTocelsius = "Fahrenheit & Celsius";
            public const string Butter = "Butter";
            public const string Liquid = "Liquid";
            public const string APFlour = "AP Flour";
            public const string BreadFlour = "Bread Flour";
            public const string RolledOats = "Rolled Oats";
            public const string WhiteSugar = "Granulated Sugar";
            public const string PackedBrownSugar = "Packed brown sugar";
            public const string Honey = "Honey";
        }

        public class Formulas
        {
            public static class LeftToRight
            {
                public static readonly Func<double, double> FahrenheitToCelcius = (double fahrenheit) => (fahrenheit - 32) * 5/9;
                public static readonly Func<double, double> ButterStickToGrams = (double butterSticks) => butterSticks * 113;
                public static readonly Func<double, double> CupToMl = (double cup) => cup * 237;
                public static readonly Func<double, double> APFlourCupsToGrams = (double cup) => cup * 128;
                public static readonly Func<double, double> BreadFlourCupsToGrams = (double cup) => cup * 136;
                public static readonly Func<double, double> OatsCupsToGrams = (double cup) => cup * 85;
                public static readonly Func<double, double> WhiteSugarCupsToGrams = (double cup) => cup * 201;
                public static readonly Func<double, double> PackedBrownSugarCupsToGrams = (double cup) => cup * 220;
                public static readonly Func<double, double> HoneyCupsToGrams = (double cup) => cup * 340;
            }

            public class RightToLeft
            {
                public static readonly Func<double, double> CelciusToFahrenheit = (double celscius) => (celscius * 9/5) + 32;
                public static readonly Func<double, double> GramsToButterStick = (double grams) => grams / 113;
                public static readonly Func<double, double> MlToCup= (double ml) => ml / 237;
                public static readonly Func<double, double> GramsToAPFlourCups = (double grams) => grams / 128;
                public static readonly Func<double, double> GramsToBreadFlourCups = (double grams) => grams / 136;
                public static readonly Func<double, double> GramsToOatsCups = (double grams) => grams / 85;
                public static readonly Func<double, double> GramsToWhiteSugarCups = (double grams) => grams / 201;
                public static readonly Func<double, double> GramsToPackedBrownSugarCups = (double grams) => grams / 220;
                public static readonly Func<double, double> GramsToHoneyCups = (double grams) => grams / 340;
            }

        }
    }
}
