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
            };

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // Add handler for ContentFrame navigation.
            ContentFrame.Navigated += On_Navigated;

            // NavView doesn't load any page by default, so load home page.
            NavView.SelectedItem = NavView.MenuItems[0];
            // If navigation occurs on SelectionChanged, this isn't needed.
            // Because we use ItemInvoked to navigate, we need to call Navigate
            // here to load the home page.
            NavView_Navigate("home", new EntranceNavigationTransitionInfo());

            // Add keyboard accelerators for backwards navigation.
            var goBack = new KeyboardAccelerator { Key = VirtualKey.GoBack };
            goBack.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(goBack);

            // ALT routes here
            var altLeft = new KeyboardAccelerator
            {
                Key = VirtualKey.Left,
                Modifiers = VirtualKeyModifiers.Menu
            };
            altLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(altLeft);
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
                ((RecipePage)ContentFrame.Content).RecipeNameUpdated += RecipeNameUpdated;
            }
            else
            {
                RecipeNavigationHelper.GetRecipeNavigationHelper().InvalidateCurrentRecipeSelection();
                ContentFrame.Navigate(page, null, transitionInfo);
            }
        }

        private void NavView_BackRequested(muxc.NavigationView sender,
                                           muxc.NavigationViewBackRequestedEventArgs args)
        {
            On_BackRequested();
        }

        private void BackInvoked(KeyboardAccelerator sender,
                                 KeyboardAcceleratorInvokedEventArgs args)
        {
            On_BackRequested();
            args.Handled = true;
        }

        private bool On_BackRequested()
        {
            if (!ContentFrame.CanGoBack)
                return false;

            // Don't go back if the nav pane is overlayed.
            if (NavView.IsPaneOpen &&
                (NavView.DisplayMode == muxc.NavigationViewDisplayMode.Compact ||
                 NavView.DisplayMode == muxc.NavigationViewDisplayMode.Minimal))
                return false;

            ContentFrame.GoBack();
            return true;
        }

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            NavView.IsBackEnabled = ContentFrame.CanGoBack;

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

        private void AddRecipe(object sender, RoutedEventArgs e)
        {
            var recipeNavHelper = RecipeNavigationHelper.GetRecipeNavigationHelper();

            var recipeNavRef = new RecipeNavigationReference()
            {
                Id = Guid.NewGuid(),
                Title = "New Recipe",
                Tag = $"recipe_{ recipeNavHelper.GetNextRecipeId() }"
            };

            NavView.MenuItems.Add(new muxc.NavigationViewItem
            {
                Content = recipeNavRef.Title,
                Icon = new SymbolIcon((Symbol)0xED56),
                Tag = recipeNavRef.Tag
            });

            recipeNavHelper.AddRecipeNavigationReference(recipeNavRef);
            _pages.Add((recipeNavRef.Tag, typeof(RecipePage)));
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
