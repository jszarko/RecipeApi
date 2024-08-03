namespace RecipeApi.Models
{
	public class SearchRecipeByTextRequest
	{
		public string? SearchString { get; set; }
		public long? CategoryId { get; set; }
		public int PageNumber {  get; set; }
		public int RecordsPerPage { get; set; }
	}
}
