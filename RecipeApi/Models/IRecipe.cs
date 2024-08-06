namespace RecipeApi.Models
{
	public interface IRecipe
	{
		int Id { get; set; }
		int? Category { get; set; }
		string? Name { get; set; }
		string? Image { get; set; }
		string? Description { get; set; }
		string? Ingredients { get; set; }
		string? Instructions { get; set; }
		DateTime? AddDate { get; set; }
		DateTime? ChangeDate { get; set; }
	}
}
