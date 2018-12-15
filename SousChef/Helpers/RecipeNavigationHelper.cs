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

        public void InvalidateCurrentRecipeSelection() => currentRecipeNavRef = null;
        public void SetCurrentRecipePage(RecipeNavigationReference currentRecipe) => currentRecipeNavRef = currentRecipe;
        public void SetCurrentRecipePageUsingTag(string tag) => currentRecipeNavRef = recipeNavigationReferences.FirstOrDefault(x => x.Tag.Equals(tag));

        public RecipeNavigationReference GetCurrentRecipePage() => currentRecipeNavRef;

        public void AddRecipeNavigationReference(RecipeNavigationReference navigationRef) => recipeNavigationReferences.Add(navigationRef);

        public bool IsRecipeWithIdOpen(Guid recipeId) => recipeNavigationReferences.Any(x => x.Id == recipeId);
        public string GetRecipeTagForId(Guid recipeId) => recipeNavigationReferences.FirstOrDefault(x => x.Id == recipeId)?.Tag;

        public int GetRecipeCount() => recipeNavigationReferences.Count();
        public int GetNextRecipeId() => GetRecipeCount() + 1;

        public void UpdateRecipeName(Guid recipeGuid, string newName) => recipeNavigationReferences.FirstOrDefault(x => x.Id.Equals(recipeGuid)).Title = newName;
    }
}

