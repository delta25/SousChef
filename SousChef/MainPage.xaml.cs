using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
using Windows.System;
using muxc = Microsoft.UI.Xaml.Controls;
using SousChef.Pages;
using SousChef.Models;
using SousChef.Helpers;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SousChef
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region properties

        double defaultCompactModeThreshold;

        #endregion

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        // List of ValueTuple holding the Navigation Tag and the relative Navigation Page
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
            {
                ("home", typeof(HomePage)),
                ("timers", typeof(TimersListingPage)),
                ("myRecipes", typeof(RecipeListing)),
            };

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            defaultCompactModeThreshold = NavView.CompactModeThresholdWidth;
            // Add handler for ContentFrame navigation.
            ContentFrame.Navigated += On_Navigated;

            NavView.SelectedItem = NavView.MenuItems[0];
            NavView_Navigate("home", new EntranceNavigationTransitionInfo());

            /* ALT routes here
            var altLeft = new KeyboardAccelerator
            {
                Key = VirtualKey.Left,
                Modifiers = VirtualKeyModifiers.Menu
            };
            altLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(altLeft);*/

            NavView.IsBackButtonVisible = muxc.NavigationViewBackButtonVisible.Collapsed;

        }

        private void NavView_ItemInvoked(muxc.NavigationView sender, muxc.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                NavView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            Type page = null;
            if (navItemTag == "settings")
            {
                page = typeof(SettingsPage);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                page = item.Page;
            }

            if (page == typeof(RecipePage))
            {
                // Don't allow navigating to the same recipe
                if (RecipeNavigationHelper.GetRecipeNavigationHelper().GetCurrentRecipePage()?.Tag == navItemTag) return;

                RecipeNavigationHelper.GetRecipeNavigationHelper().SetCurrentRecipePageUsingTag(navItemTag);
                var recipeId = RecipeNavigationHelper.GetRecipeNavigationHelper().GetCurrentRecipePage().Id;

                ContentFrame.Navigate(page, recipeId, transitionInfo);
                NavView.IsPaneOpen = false;
                ((RecipePage)ContentFrame.Content).RecipeNameUpdatedEvent += RecipeNameUpdated;
            }
            else if (page == typeof(RecipeListing))
            {
                RecipeNavigationHelper.GetRecipeNavigationHelper().InvalidateCurrentRecipeSelection();
                ContentFrame.Navigate(page, null, transitionInfo);

                ((RecipeListing)ContentFrame.Content).RecipeRestoredEvent += RestoreRecipe;
            }
            else
            {
                RecipeNavigationHelper.GetRecipeNavigationHelper().InvalidateCurrentRecipeSelection();
                ContentFrame.Navigate(page, null, transitionInfo);
            }
        }

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            if (ContentFrame.SourcePageType == typeof(SettingsPage))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                NavView.SelectedItem = (muxc.NavigationViewItem)NavView.SettingsItem;
                NavView.Header = "Settings";
            }
            else if (ContentFrame.SourcePageType == typeof(RecipePage))
            {
                NavView.SelectedItem = NavView.MenuItems
                    .OfType<muxc.NavigationViewItem>()
                    .First(n => n.Tag.Equals(RecipeNavigationHelper.GetRecipeNavigationHelper().GetCurrentRecipePage().Tag));
            }
            else if (ContentFrame.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

                NavView.SelectedItem = NavView.MenuItems
                    .OfType<muxc.NavigationViewItem>()
                    .First(n => n.Tag.Equals(item.Tag));

                NavView.Header = ((muxc.NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();
            }
        }

        #region Recipe events

        private void RestoreRecipe(RecipeCache recipeCache)
        {
            RecipeCachingHelper.cache[recipeCache.Id] = recipeCache;

            AddRecipe(new RecipeNavigationReference()
            {
                Id = recipeCache.Id,
                Title = recipeCache.Name
            });
        }

        private void AddRecipe(object sender, RoutedEventArgs e)
        {
            var recipeNavRef = new RecipeNavigationReference()
            {
                Id = Guid.NewGuid(),
                Title = "New Recipe",
            };

            AddRecipe(recipeNavRef);
        }

        private void AddRecipe(RecipeNavigationReference recipeNavRef)
        {
            var recipeNavHelper = RecipeNavigationHelper.GetRecipeNavigationHelper();
            if (recipeNavHelper.IsRecipeWithIdOpen(recipeNavRef.Id))
            {
                NavView_Navigate(recipeNavHelper.GetRecipeTagForId(recipeNavRef.Id), null);
                return;
            }

            recipeNavRef.Tag = $"recipe_{ recipeNavHelper.GetNextRecipeId() }";

            NavView.MenuItems.Add(new muxc.NavigationViewItem
            {
                Content = recipeNavRef.Title,
                Icon = new SymbolIcon((Symbol)0xED56),
                Tag = recipeNavRef.Tag
            });

            recipeNavHelper.AddRecipeNavigationReference(recipeNavRef);
            _pages.Add((recipeNavRef.Tag, typeof(RecipePage)));

            NavView_Navigate(recipeNavRef.Tag, null);
        }

        private void RecipeNameUpdated(Guid recipeGuid, string newName)
        {
            RecipeNavigationHelper.GetRecipeNavigationHelper().UpdateRecipeName(recipeGuid, newName);

            NavView.MenuItems
                   .OfType<muxc.NavigationViewItem>()
                   .First(n => n.Tag.Equals(RecipeNavigationHelper.GetRecipeNavigationHelper().GetCurrentRecipePage().Tag)).Content = newName;
        }

        #endregion
    }
}
