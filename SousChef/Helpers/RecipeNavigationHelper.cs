using SousChef.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SousChef.Helpers
{
    public class RecipeNavigationHelper
    {
        private static RecipeNavigationHelper recipeNavigationHelper = new RecipeNavigationHelper();

        private RecipeNavigationReference currentRecipeNavRef;
        private List<RecipeNavigationReference> recipeNavigationReferences = new List<RecipeNavigationReference>();

        public static RecipeNavigationHelper GetRecipeNavigationHelper() => recipeNavigationHelper;

        public void SetCurrentRecipePage(RecipeNavigationReference currentRecipe) => currentRecipeNavRef = currentRecipe;
        public void SetCurrentRecipePage(string tag) => currentRecipeNavRef = recipeNavigationReferences.FirstOrDefault(x => x.Tag.Equals(tag));

        public RecipeNavigationReference GetCurrentRecipePage() => currentRecipeNavRef;

        public void AddRecipeNavigationReference(RecipeNavigationReference navigationRef) => recipeNavigationReferences.Add(navigationRef);

        public int GetRecipeCount() => recipeNavigationReferences.Count();
        public int GetNextRecipeId() => recipeNavigationReferences.Count() +1;        
    }
}
