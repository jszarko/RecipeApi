namespace RecipeApi.Models
{
	public class SearchRecipeByTextRequest
	{
		public string? SearchString { get; set; }
		public int? CategoryId { get; set; }
		public int PageNumber {  get; set; }
		public int RecordsPerPage { get; set; }
	}
}
