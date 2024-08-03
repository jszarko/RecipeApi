using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace RecipeApi.Models;

public class RecipeContext : DbContext
{
	public RecipeContext(DbContextOptions<RecipeContext> options)
		: base(options)
	{
	}

	public DbSet<Recipe> Recipes { get; set; } = null!;
	public DbSet<PagedRecipe> PagedRecipes { get; set; } = null!;
	public DbSet<Category> Categories { get; set; } = null!;
}