using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeApi.Models
{
	public class PagedRecipe : IRecipe
	{
		public long Id { get; set; }
		public long? Category { get; set; }
		public string? Name { get; set; }
		public string? Image { get; set; }
		public string? Description { get; set; }
		public string? Ingredients { get; set; }
		public string? Instructions { get; set; }
		[Column("add_date")]
		public DateTime? AddDate { get; set; }
		[Column("change_date")]
		public DateTime? ChangeDate { get; set; }
		[Column("total_pages")]
		public int? TotalPages { get; set; }
	}
}
