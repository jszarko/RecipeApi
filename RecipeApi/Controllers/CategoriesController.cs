using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeApi.Models;

namespace RecipeApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoriesController : ControllerBase
	{
		private readonly RecipeContext _context;
		

		public CategoriesController(RecipeContext context)
		{
			_context = context;			
		}

		// GET: api/Categories
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
		{
			return await _context.Categories.ToListAsync();
		}

		// GET: api/Categories/5
		[HttpGet("{id}")]
		public async Task<ActionResult<Category>> GetCategories(int id)
		{
			var category = await _context.Categories.FindAsync(id);

			if (category == null)
			{
				return NotFound();
			}

			return category;
		}
	}
}
