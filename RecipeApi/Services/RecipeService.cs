using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RecipeApi.Models;

namespace RecipeApi.Services
{
	public class RecipeService
	{
		private readonly RecipeContext _recipeContext;

		public RecipeService(RecipeContext recipeContext)
		{
			_recipeContext = recipeContext;
		}

		public async Task<IEnumerable<PagedRecipe>> SearchRecipesByText(string searchText, int? categoryId, int pageNumber, int recordsPerPage)
		{
			var searchParam = new SqlParameter("@searchText", SqlDbType.NVarChar).Value = searchText;
			var categoryIdParam = categoryId is null ? DBNull.Value : new SqlParameter("@categoryId", SqlDbType.BigInt).Value = categoryId;
			var pageNumberParam = new SqlParameter("@pageNumber", SqlDbType.Int).Value = pageNumber;
			var recordsPerPageParam = new SqlParameter("@recordsPerPage", SqlDbType.Int).Value = recordsPerPage;
			return await _recipeContext.PagedRecipes
				.FromSqlRaw("EXEC [dbo].[SearchRecipesByText] {0}, {1}, {2}, {3}", searchParam, categoryIdParam, pageNumberParam, recordsPerPageParam)
				.ToListAsync<PagedRecipe>();
		}

		public async Task<IEnumerable<Recipe>> GetLatestRecipes(int count)
		{
			var top5Results = await _recipeContext.Recipes
				.OrderByDescending(r => r.AddDate)
				.Take(count)
				.ToListAsync<Recipe>();
			return top5Results;
		}

		public async Task<IEnumerable<Recipe>> GetRecipesByCategory(int categoryId)
		{
			var results = await _recipeContext.Recipes
				.Where(r => r.Category == categoryId)
				.ToListAsync<Recipe>();
			return results;
		}

	}
}
