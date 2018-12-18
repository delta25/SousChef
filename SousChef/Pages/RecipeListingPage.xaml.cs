using Newtonsoft.Json;
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
    public sealed partial class RecipeListingPage : Page
    {
        public delegate void RecipeRestoredHandler(RecipeCache recipeCache);
        public RecipeRestoredHandler RecipeRestoredEvent;

        public RecipeListingPage()
        {
            this.InitializeComponent();

            LoadRecipesFromDisk();
        }

        private async void LoadRecipesFromDisk()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            var recipes = (await storageFolder.GetFilesAsync()).Where(x => x.Name.EndsWith(".rcp")).ToList();
            var recipeFileContents = await Task.WhenAll(recipes.Select(async x => await FileIO.ReadTextAsync(x)));

            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var availableRecipes = recipeFileContents.Select(x => JsonConvert.DeserializeObject<RecipeCache>(x, settings))
                                                        .Where(x => x.IsFavourite)
                                                        .OrderBy(x => x.Name)
                                                        .ToList();

            recipeListing.ItemsSource = availableRecipes;
        }

        private void RecipeDoubleClicked(object sender, DoubleTappedRoutedEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext;
            var myRecipe = item as RecipeCache;
            if (item != null)
                RecipeRestoredEvent?.Invoke(myRecipe);
        }
    }
}
