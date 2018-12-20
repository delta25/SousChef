using Newtonsoft.Json;
using SousChef.Constants;
using SousChef.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SousChef.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConvertersPage : Page
    {

        public UnitConversion selectedConverter;

        public ConvertersPage()
        {
            this.InitializeComponent();

            //PopulateConverterOptions();
            LoadConvertersAsync();
        }

        private async Task LoadConvertersAsync()
        {
            

                //var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Resources/Converters.csv"));

        }

        private void PopulateConverterOptions()
        {
            var fahrenheitTocelsius = new IngredientConversion()
            {
                Name = IngredientConversions.Names.FahrenheitTocelsius,
                FormulaLeftToRight = IngredientConversions.Formulas.LeftToRight.FahrenheitToCelcius,
                FormulaRightToLeft = IngredientConversions.Formulas.RightToLeft.CelciusToFahrenheit
            };

            var butter = new IngredientConversion()
            {
                Name = IngredientConversions.Names.Butter,
                FormulaLeftToRight = IngredientConversions.Formulas.LeftToRight.ButterStickToGrams,
                FormulaRightToLeft = IngredientConversions.Formulas.RightToLeft.GramsToButterStick
            };

            var liquid = new IngredientConversion()
            {
                Name = IngredientConversions.Names.Liquid,
                FormulaLeftToRight = IngredientConversions.Formulas.LeftToRight.CupToMl,
                FormulaRightToLeft = IngredientConversions.Formulas.RightToLeft.MlToCup
            };

            var aPFlour = new IngredientConversion()
            {
                Name = IngredientConversions.Names.APFlour,
                FormulaLeftToRight = IngredientConversions.Formulas.LeftToRight.APFlourCupsToGrams,
                FormulaRightToLeft = IngredientConversions.Formulas.RightToLeft.GramsToAPFlourCups
            };

            var breadFlour = new IngredientConversion()
            {
                Name = IngredientConversions.Names.BreadFlour,
                FormulaLeftToRight = IngredientConversions.Formulas.LeftToRight.BreadFlourCupsToGrams,
                FormulaRightToLeft = IngredientConversions.Formulas.RightToLeft.GramsToBreadFlourCups
            };

            var rolledOats = new IngredientConversion()
            {
                Name = IngredientConversions.Names.RolledOats,
                FormulaLeftToRight = IngredientConversions.Formulas.LeftToRight.OatsCupsToGrams,
                FormulaRightToLeft = IngredientConversions.Formulas.RightToLeft.GramsToOatsCups
            };

            var whiteSugar = new IngredientConversion()
            {
                Name = IngredientConversions.Names.WhiteSugar,
                FormulaLeftToRight = IngredientConversions.Formulas.LeftToRight.WhiteSugarCupsToGrams,
                FormulaRightToLeft = IngredientConversions.Formulas.RightToLeft.GramsToWhiteSugarCups
            };

            var packedBrownSugar = new IngredientConversion()
            {
                Name = IngredientConversions.Names.PackedBrownSugar,
                FormulaLeftToRight = IngredientConversions.Formulas.LeftToRight.PackedBrownSugarCupsToGrams,
                FormulaRightToLeft = IngredientConversions.Formulas.RightToLeft.GramsToPackedBrownSugarCups
            };

            var honey = new IngredientConversion()
            {
                Name = IngredientConversions.Names.Honey,
                FormulaLeftToRight = IngredientConversions.Formulas.LeftToRight.HoneyCupsToGrams,
                FormulaRightToLeft = IngredientConversions.Formulas.RightToLeft.GramsToHoneyCups
            };

            var unitConversionList = new List<UnitConversion>
            {
                new UnitConversion()
                {
                    Name = UnitConversions.Names.Temperature,
                    LeftImage = UnitConversions.Icons.Fahrenheit,
                    RightImage = UnitConversions.Icons.Celsius,
                    IngredientConversions = new List<IngredientConversion>() { fahrenheitTocelsius }
                },
                new UnitConversion()
                {
                    Name = UnitConversions.Names.ButterAndGrams,
                    LeftImage = UnitConversions.Icons.ButterStick,
                    RightImage = UnitConversions.Icons.Weight,
                    IngredientConversions = new List<IngredientConversion>() { butter }
                },
                new UnitConversion()
                {
                    Name = UnitConversions.Names.CupsAndML,
                    LeftImage = UnitConversions.Icons.ButterStick,
                    RightImage = UnitConversions.Icons.Weight,
                    IngredientConversions = new List<IngredientConversion>() { liquid }
                },
                new UnitConversion()
                {
                    Name = UnitConversions.Names.CupsAndGrams,
                    LeftImage = UnitConversions.Icons.ButterStick,
                    RightImage = UnitConversions.Icons.Weight,
                    IngredientConversions = new List<IngredientConversion>() { aPFlour, breadFlour, rolledOats, whiteSugar, packedBrownSugar, honey }
                }
            };

            converterListing.ItemsSource = unitConversionList;
        }
    }

}