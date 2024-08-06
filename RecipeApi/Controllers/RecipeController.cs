using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeApi.Models;
using RecipeApi.Services;

namespace RecipeApi.Controllers
{
    [Route("api/Recipe")]
    [ApiController]
    public class RecipesController : ControllerBase
	{
		private readonly RecipeContext _context;
		private readonly RecipeService _recipeService;
		private static bool prePopulated = false;

		public RecipesController(RecipeContext context, RecipeService recipeService)
		{
			_context = context;
			_recipeService = recipeService;
			// no longer in use, now using database directly
			// prePopulateDatabase(_context);
		}

		// GET: api/Recipe
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes()
		{
			return await _context.Recipes.ToListAsync();
		}

		// GET: api/Recipe/5
		[HttpGet("{id}")]	
		public async Task<ActionResult<Recipe>> GetRecipes(int id)
		{
			var recipe = await _context.Recipes.FindAsync(id);

			if (recipe == null)
			{
				return NotFound();
			}

			return recipe;
		}

		// GET: api/Recipe/latest?count=5
		[HttpGet]
		[Route("latest")]
		public async Task<ActionResult<IEnumerable<Recipe>>> GetLatestRecipes([FromQuery] int count)
		{
			var recipes = await _recipeService.GetLatestRecipes(count);

			if (recipes == null)
			{
				return NotFound();
			}

			return Ok(recipes); ;
		}

		// GET: api/Recipe/category?category=18
		[HttpGet]
		[Route("category")]
		public async Task<ActionResult<Recipe>> GetRecipesByCategory([FromQuery] int category)
		{
			var recipes = await _recipeService.GetRecipesByCategory(category);

			if (recipes == null)
			{
				return NotFound();
			}

			return Ok(recipes); ;
		}

		// PUT: api/Recipe/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkId=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutRecipe(int id, Recipe recipe)
		{
			if (id != recipe.Id)
			{
				return BadRequest();
			}

			_context.Entry(recipe).State = EntityState.Modified;

			try
			{
				if (recipe.ChangeDate is null)
					recipe.ChangeDate = DateTime.Now;
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!RecipeExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		// POST: api/Recipe
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkId=2123754
		[HttpPost]
		public async Task<ActionResult<Recipe>> PostRecipe(Recipe recipe)
		{
			if (recipe.AddDate is null)
				recipe.AddDate = DateTime.Now;

			_context.Recipes.Add(recipe);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(PostRecipe), new { Id = recipe.Id }, recipe);
		}

		// DELETE: api/Recipe/5
		[HttpDelete("{Id}")]
		public async Task<IActionResult> DeleteRecipe(int id)
		{
			var recipe = await _context.Recipes.FindAsync(id);
			if (recipe == null)
			{
				return NotFound();
			}

			_context.Recipes.Remove(recipe);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool RecipeExists(int id)
		{
			return _context.Recipes.Any(e => e.Id == id);
		}

		[HttpPost]
		[Route("search")]
		public async Task<ActionResult<IEnumerable<PagedRecipe>>> SearchRecipesByText([FromBody] SearchRecipeByTextRequest searchRecipeByTextRequest)
		{
			var recipes = await _recipeService.SearchRecipesByText(
				searchRecipeByTextRequest.SearchString, 
				searchRecipeByTextRequest.CategoryId, 
				searchRecipeByTextRequest.PageNumber, 
				searchRecipeByTextRequest.RecordsPerPage);

			if (recipes == null)
			{
				return NotFound();
			}

			return Ok(recipes);
		}

	// UPDATE: no longer in use, now using database directly
	// temporary solution until database is created and functioning
	private void prePopulateDatabase(RecipeContext recipeContext)
	{
		if (!prePopulated)
		{
			recipeContext.Recipes.AddRange(
				new List<Recipe>
				{
					new Recipe
					{
						Category = 5,
						Name = "Chewy Brown Sugar Cookies",
						Description = "Super soft and chewy brown sugar cookies - no mixer required.",
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/5d/SugarCookie.JPG/640px-SugarCookie.JPG",
						Ingredients = "2 cups (250g) all-purpose flour (spooned & leveled)\n1 teaspoon baking soda\n1 and 1/2 teaspoons cornstarch\n1/2 teaspoon ground cinnamon (use 1 teaspoon if you love cinnamon)\n1/4 teaspoon salt\n3/4 cup (12 Tbsp; 170g) unsalted butter, melted and slightly cooled\n1 and 1/4 cups (250g) packed light or dark brown sugar\n1 large egg, room temperature\n2 teaspoons pure vanilla extract\n1/3 cup (67g) granulated sugar, for rolling",
						Instructions = "Toss together the flour, baking soda, cornstarch, cinnamon, and salt in a large bowl. Set asIde.\n\nIn a medium size bowl, whisk the melted butter and brown sugar together until no brown sugar lumps remain. Whisk in the egg. Finally, whisk in the vanilla. Pour the wet Ingredients into the dry Ingredients and mix together with a large spoon or rubber spatula. The dough will be very soft, yet thick. Cover the dough and chill for 2 hours, or up to 3 days. Chilling is mandatory.\n\nTake the dough out of the refrigerator and allow to slightly soften at room temperature for 10 minutes if you had it chilling for more than 2 hours.\n\nPreheat the oven to 325°F (163°C). Line two large baking sheets with parchment paper or silicone baking mats. Set asIde.\n\nPour the granulated sugar into a bowl. Take 2 scant tablespoons of dough and roll into a ball, then roll into the sugar. Place 3 inches apart on the baking sheets.\n\nBake for 8-9 minutes. Remove from the oven and gently press the top of the cookie down with the back of a utensil or even use your fingers. You’re trying to obtain a crinkly top. Place back into the oven for 2-4 more minutes. The total time these cookies are in the the oven is 10-13 minutes. The cookies will be puffy and still appear very soft in the mIddle. Remove from the oven and allow to cool on the baking sheet for ten minutes before transferring to a wire rack to cool completely. They will continue to cook in the center on the baking sheet after being removed from the oven.\n\nCookies will stay fresh covered at room temperature for 1 week.",
					},
					new Recipe
					{
						Category = 5,
						Name = "Classic Cheesecake",
						Description = "A classic for a reason, this cheesecake is silky smooth and luxurious. Paired with a buttery graham cracker crust, no one can deny its simple decadence.",
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/de/My_first_cheesecake_cropped.jpg/220px-My_first_cheesecake_cropped.jpg",
						Ingredients = "Graham Cracker Crust:\n1 1/2 cups (180g) graham cracker crumbs (about 12 full sheet graham crackers)\n1/4 cup (50g) granulated sugar\n5 tablespoons (71g) unsalted butter, melted\n\nCheesecake Filling:\n32 ounces (904g) full-fat cream cheese, softened to room temperature\n1 cup (200g) granulated sugar\n1 cup (240g) full-fat sour cream, at room temperature\n1 teaspoon pure vanilla extract\n2 teaspoons fresh lemon juice (optional, but recommended)\n3 large eggs, at room temperature",
						Instructions = "Adjust the oven rack to the lower-mIddle position and preheat oven to 350°F (177°C).\n\nMake the crust: If you’re starting out with full graham crackers, use a food processor or blender to grind them into fine crumbs. Pour into a medium bowl and stir in sugar until combined, and then stir in the melted butter. Mixture will be sandy. Try to smash/break up any large chunks. Pour into an ungreased 9-inch or 10-inch springform pan. With medium pressure using your hand, pat the crumbs down into the bottom and partly up the sIdes to make a compact crust. Do not pack down with heavy force because that makes the crust too hard. Simply pat down until the mixture is no longer crumby/crumbly and you can use the flat bottom of a small measuring cup to help smooth it all out if needed. Pre-bake for 10 minutes. Remove from the oven and place the hot pan on a large piece of aluminum foil. The foil will wrap around the pan for the water bath. Allow crust to slightly cool as you prepare the filling.\n\nMake the filling: Using a handheld or stand mixer fitted with a paddle attachment, beat the cream cheese and granulated sugar together on medium-high speed in a large bowl until the mixture is smooth and creamy, about 2 minutes. Add the sour cream, vanilla extract, and lemon juice then beat until fully combined. On medium speed, add the eggs one at a time, beating after each addition until just blended. After the final egg is incorporated into the batter, stop mixing. To help prevent the cheesecake from deflating and cracking as it cools, avoId over-mixing the batter as best you can. You will have close to 6 cups of batter.\n\nPrepare the simple water bath: Boil a pot of water. You need 1 inch of water in your roasting pan for the water bath, so make sure you boil enough. I use an entire kettle of hot water. As the water is heating up, wrap the aluminum foil around the springform pan. Place the pan insIde of a large roasting pan. Pour the cheesecake batter on top of the crust. Use a rubber spatula or spoon to smooth it into an even layer. Carefully pour the hot water insIde of the pan and place in the oven. (Or you can place the roasting pan in the oven first, then pour the hot water in. Whichever is easier for you.)\n\nBake cheesecake for 55–70 minutes or until the center is almost set. If you notice the cheesecake browning too quickly on top, tent it with aluminum foil halfway through baking. When it’s done, the center of the cheesecake will slightly wobble if you gently shake the pan. Turn the oven off and open the oven door slightly. Let the cheesecake sit in the oven in the water bath as it cools down for 1 hour. Remove from the oven and water bath, then cool cheesecake completely uncovered at room temperature. Then cover and refrigerate the cheesecake for at least 4 hours or overnight.\n\nUse a knife to loosen the chilled cheesecake from the rim of the springform pan, then remove the rim. Using a clean sharp knife, cut into slices for serving. For neat slices, wipe the knife clean and dip into warm water between each slice.\n\nServe cheesecake with desired toppings. Cover and store leftover cheesecake in the refrigerator for up to 5 days.",
					},
					new Recipe
					{
						Category = 5,
						Name = "Matcha Cookies",
						Description = "Enjoy your afternoon tea with these crisp and buttery Matcha Cookies. The unique flavor combination of matcha and white chocolate is surprisingly delightful.",
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/cf/Matcha_cookies.jpg/640px-Matcha_cookies.jpg",
						Ingredients = "2 cups all-purpose flour \n2½ Tbsp matcha green tea powder (1 Tbsp matcha is 6 g) \n¾ cup unsalted butter (softened, at room temperature) \n1 pinch Diamond Crystal kosher salt \n130 g confectioners’ sugar (1 cup + 2 tsp) \n2 large egg yolks (at room temperature) \n¼ cup white chocolate chips",
						Instructions = "Combine 2 cups all-purpose flour and 2½ Tbsp matcha green tea powder in a large bowl. Sift the flour and the matcha powder. \n\nIn a stand mixer with a paddle attachment or in a large bowl with a hand mixer, beat ¾ cup unsalted butter until smooth and creamy. \nTip: It’s important to soften the butter ahead of time. Leave the butter out on the counter for 1 hour or microwave it in 5-second increments until it‘s softened.\nAdd 1 pinch Diamond Crystal kosher salt and blend.\nAdd 130 g confectioners’ sugar (1 cup + 2 tsp) and beat well until soft and light.\nAs you blend, stop the mixer and scrape down the bowl occasionally.\nAdd 2 large egg yolks and mix well until combined.\nGradually add the flour and matcha mixture and mix until just combined.\nAdd ¼ cup white chocolate chips and mix until just incorporated.\nDivIde the dough into 2 equal pieces.\nShape each piece into a cylinder about 1½ inches (4 cm) in diameter and 7 inches (18 cm) long.\nWrap the logs in plastic wrap and chill in the refrigerator until firm, at least 2 hours. \nTip: You can place the logs on a bed of uncooked rice while chilling. It’ll keep the dough in a nice cylindrical shape so your cookie slices won’t be flat on one sIde.\nTo Freeze for Later: You can also freeze the unbaked logs of dough, wrapped in plastic wrap, for up to 2 months. To bake, let sit at room temperature for about 10 minutes before cutting and baking. Do not let the dough fully defrost.\n\nPreheat the oven to 350ºF (175ºC). For a convection oven, reduce the cooking temperature by 25ºF (15ºC). \n\nLine a baking sheet with parchment paper or a silicone baking liner. Remove the dough from the refrigerator and unwrap the plastic wrap. Use a sharp knife to slice the dough into rounds about ⅓ inch (7 mm) thick. If the dough is too hard to slice, wait 5 minutes or so before slicing. Place the sliced dough on the baking sheet, leaving about 1 inch (2.5 cm) of space between the rounds.\n\nBake the cookies at 350ºF (175ºC) for about 15 minutes, or until the edges of the cookies start to get slightly golden brown.\n\nRemove from the oven and let the cookies cool on the baking sheet for 5 minutes; then carefully transfer the cookies to a wire cooling rack and let them cool completely before serving.\n\nYou can keep the cooled cookies in an airtight container and store them at room temperature for at least 4 days.",
					},
					new Recipe
					{
						Category = 1,
						Name = "Cranberry Nut Bread",
						Description = "Sweet, orange-scented and chock-full of cranberries and walnuts, this cranberry nut bread is perfect for the holIdays.",
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2d/Cranberry_orange_nut_bread_%2829307632738%29.jpg/640px-Cranberry_orange_nut_bread_%2829307632738%29.jpg",
						Ingredients = "⅔ cup buttermilk\n2 teaspoons grated orange zest (from 1 orange)\n⅓ cup orange juice (from 1 orange)\n6 tablespoons unsalted butter, melted\n1 large egg\n2 cups all-purpose flour, spooned into measuring cup and leveled-off\n1 cup plus 2 tablespoons sugar\n¾ teaspoon salt\n1 teaspoon ground cinnamon\n1 teaspoon baking powder\n¼ teaspoon baking soda\n1 cup fresh or frozen cranberries, halved\n½ cup coarsely chopped walnuts or pecans",
						Instructions = "Preheat oven to 375°F and set an oven rack to the mIddle position.\n\nSpray a 9 x 5-inch loaf pan with non-stick cooking spray.\n\nIn a small bowl, stir together buttermilk, orange zest and juice, melted butter and egg. Set asIde.\n\nIn a large bowl, whisk together flour, sugar, salt, cinnamon, baking powder and baking soda.\n\nStir the liquId Ingredients into the dry Ingredients with rubber spatula until just moistened.\n\nGently stir in cranberries and nuts. Do not overmix.\n\nScrape the batter into the prepared loaf pan and spread evenly with a rubber spatula.\n\nBake for 20 minutes, then reduce the heat to 350° F. Continue to bake until golden brown and a toothpick inserted into center of the loaf comes out clean, about 45 minutes longer.\n\nCool the loaf in the pan for about 10 minutes, then turn out onto the rack and cool at least 30 minutes before serving.",
					},
					new Recipe
					{
						Category = 5,
						Name = "Tiramisu",
						Description = "Tiramisu is a timeless no-bake Italian dessert combining espresso-dipped ladyfingers and a creamy lightly sweetened mascarpone cream.",
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/58/Tiramisu_-_Raffaele_Diomede.jpg/640px-Tiramisu_-_Raffaele_Diomede.jpg",
						Ingredients = "Ladyfinger Layers (or 30-40 Italian ladyfingers)\n6 eggs, separated\n3/4 cup sugar, divIded\n1 1/2 cups all-purpose flour\n1/4 tsp salt\n1 tsp pure vanilla extract\n\nMascarpone Filling\n6 extra large egg yolks, at room temperature\n1/4 cup sugar\n1/4 cup good dark rum\n1/4 cup brewed espresso or strong coffee, cooled\n16 to 17 ounces mascarpone cheese\n\nSoaking LiquId\n1/4 cup good dark rum\n1 1/4 cups brewed espresso or strong coffee, cooled\n\nTopping\nUnsweetened cocoa powder\nBittersweet chocolate, shaved or grated\n",
						Instructions = "Make the ladyfinger layers (if not using the ladyfingers):\nPreheat oven to 350 degrees F. Line 3 baking sheets with parchment paper. Trace the bottom of an 8×8” or 9×9” square pan or 9” round cake pan on each piece of parchment paper. Set asIde.\nIn a large bowl, whisk the egg yolks and 1/4 cup of sugar with an electric mixer until thick and pale. \nIn another large bowl, whip egg whites with the remaining ½ cup of sugar. Once stiff peaks form add vanilla and beat to combine.\nGently stir in egg yolk mixture on low speed.\nAdd flour and salt and gently stir or fold in with a spatula. The batter should be thick and pale yellow.\nPour about 1 cup of batter into the center of each square traced on the parchment paper. Evenly spread it out to fit the square, leaving about ¼” border. The batter should be about 1/2” thick.\nBake in preheated oven for 10-12 minutes until set and very lightly browned. Carefully transfer parchment paper to a wire rack to cool.\nOnce cooled, carefully remove the ladyfinger squares from the parchment paper, set asIde.\n\nMake the soaking liquId:\nCombine 1/4 cup rum and 1 1/4 cups espresso or very strong coffee in a shallow bowl.\n\nMake the filling:\nCombine the egg yolks and sugar in a heat-proof mixing bowl. Place the bowl over a pot of simmering water, ensuring that the bowl doesn’t touch the water. Continue to whisk on high until the egg yolk mixture is very thick and light yellow (this will take 5 to 8 minutes).\nMake sure that the egg yolk mixture is cool, then add 1/4 cup of rum, 1/4 cup of espresso/strong coffee and the mascarpone and whisk on medium until smooth.\n\nAssembly:\nWith the ladyfinger layers:\nQuickly dip one of the three ladyfinger layers into the soaking liquId for about 5-10 seconds.\nPlace it in the bottom of the baking dish you used to trace the shape.\nSpread 1/3 of the filling mixture evenly on top of the layer.\nRepeat with the second and third ladyfinger layers and remaining filling. Alternating soaked ladyfinger, filling, soaked ladyfinger, filling.\n\nWith the ladyfingers:\nQuickly dip one sIde of each ladyfinger in the espresso-rum mixture and line the bottom of a 9 x 12 x 2-inch dish. Pour half the espresso cream mixture evenly on top. Quickly dip one sIde of the remaining ladyfingers in the espresso-rum mixture and place them in a second layer in the dish. Pour the rest of the espresso cream over the top. \n\nSmooth the top and cover with plastic wrap. Refrigerate overnight.\n\nBefore serving, dust the top lightly with cocoa powder and sprinkle with shaved chocolate, if desired.",
					},
					new Recipe
					{
						Category = 5,
						Name = "Key Lime Pie",
						Description = "This tangy-sweet key lime pie is deliciously simple with an extra thick and nutty crust. Be sure to refrigerate the pie for at least an hour before slicing and serving.",
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d6/Andrea_made_us_key_lime_pie.jpg/640px-Andrea_made_us_key_lime_pie.jpg",
						Ingredients = "For the Crust:\n1½ cups finely crushed graham cracker crumbs (from about 12 whole graham crackers)\n⅓ cup packed light brown sugar\n4 tablespoons unsalted butter, melted\n\nFor the Filling:\n2 14-oz cans sweetened condensed milk\n1 cup plain Greek yogurt (2% or whole milk)\n1 tablespoon grated lime zest\n¾ cup fresh lime juice\n\nFor the Topping:\n1 cup cold heavy cream\n2 tablespoons confectioners' sugar\n1 teaspoon grated lime zest\n8 to 10 thin lime slices",
						Instructions = "For the Crust:\nPreheat oven to 375 °F and set an oven rack in the mIddle position.\n\nIn a medium bowl, combine the graham cracker crumbs, brown sugar, and melted butter; stir with a fork first, and then your hands until the mixture is well combined.\n\nUsing your fingers and the bottom of a glass or dry measuring cup, press the crumbs firmly into the bottom and up the sIdes of a 9 x 1.5-inch (deep-dish) pie pan. The crust should be about ¼-inch thick. (Tip: do the sIdes first.)\n\nBake for 10 minutes, until just slightly browned. Let the crust cool on a wire rack.\n\nFor the Filling:\nLower the oven temperature to 350°F. In a large bowl, whisk together the sweetened condensed milk, yogurt, lime zest, and lime juice. \n\nPour the thick mixture into the warm graham cracker crust.\n\nBake for 15 minutes, until the filling is almost set; it should wobble a bit.\n\nLet cool at room temperature for 30 minutes, then place in the refrigerator to chill thoroughly, about 3 hours.\n\nFor the Topping: In the bowl of an electric mixer, beat the heavy cream until soft peaks form. Add the confectioners’ sugar and beat until medium peaks form.\n\nTop the pie with the whipped cream.\nDecorate with the lime zest and lime slices.\nStore the pie in the refrigerator until ready to serve.\nSlice the pie into wedges, wiping your knife clean between slices, and serve cold.",
					},
					new Recipe
					{
						Category = 5,
						Name = "Japanese Cheesecake",
						Description = "Light, jiggly, and fluffy, Japanese Cheesecake (Soufflé Cheesecake) is seriously the most delicious treat to serve for a crowd. It has the melt-in-your-mouth combination of creamy cheesecake and airy soufflé.",
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/e5/%E8%BC%95%E4%B9%B3%E9%85%AA%E8%9B%8B%E7%B3%95.jpg/640px-%E8%BC%95%E4%B9%B3%E9%85%AA%E8%9B%8B%E7%B3%95.jpg",
						Ingredients = "1 Tbsp unsalted butter (for greasing the pan and parchment paper)\n6 large eggs (50 g each w/o shell) (10.6 oz, 300 g without shell)\n10.6 oz cream cheese\n4 Tbsp unsalted butter\n¾ cup heavy (whipping) cream (¾ cup + 4 tsp, to be precise)\n4½ Tbsp sugar (for the cream cheese mixture)\n⅔ cup cake flour (weigh your flour or use the “fluff and sprinkle“ method and level it off)\n½ lemon (for the zest)\n2 Tbsp lemon juice (from ½ large lemon)\n½ cup sugar (for beating the egg whites)\n2 Tbsp apricot jam (for the glaze)\n2 tsp hot water (for the glaze)",
						Instructions = "To Prepare the Cake Pan: Use a 9-inch (23-cm) cake pan that is 4 inches (10 cm) high. Cut parchment paper to line the bottom and sIdes of the cake pan. Cut one circle 9 inches (23 cm) in diameter for the bottom and one rectangular strip 4 x 30 inches (10 x 76 cm) for the sIdes of the cake pan. In addition, cut two strips of paper 2 x 30 inches (5 x 76 cm) each. We will use these as “straps” to lift the baked cake from the pan. With 1 Tbsp unsalted butter, grease the cake pan and the parchment paper (for the bottom and the sIdes only; grease the paper on one sIde). You don‘t need to use all the butter. Place the two parchment paper “straps” crisscross on the bottom of the cake pan so they form an “X.” Allow the excess paper to hang over the sIdes. Then, line the bottom and sIdes with the greased parchment paper. The greased sIde of the paper circle should face up, and the greased sIde of the rectangular strip should face in toward the center of the pan.",
					},
					new Recipe
					{
						Category = 5,
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/44/Tarte.tatin.wmt.jpg/640px-Tarte.tatin.wmt.jpg",
						Name = "Apple Tarte Tatin",
						Description = "The Tarte Tatin is a classic French dessert. It was Named after the Tatin sisters, who invented it and served it in their hotel as its signature dish. It’s a pastry in which the fruit (usually apples) is caramelized in butter and sugar before the tart is baked. ",
						Ingredients = "Puff Pastry\nsalted butter\ngranulated white sugar\nGranny Smith apples\nvanilla ice cream, for serving (optional)\n",
						Instructions = "Preheat the oven to 375℉.\nUse a 9-inch cake pan as a template, and cut a 9-inch circle from the puff pastry. Poke it all over with a fork.\nIn a large saucepan over medium-heat, combine the butter and sugar. Cook about 5 minutes, until light brown in color (stirring constantly). Add the apples, stirring until they are coated in a thick layer of caramel. Cook for about 15 to 20 minutes, turning the apples constantly so that they bathe in the caramel. Remove from the heat when the caramel has reduced and little remains in the bottom of the pan. \nPosition the apple slices in concentric circles on the bottom of the cake pan. Press the apples tightly against each other, then pour the remaining caramel over the top.\nLay the circle of puff pastry on top. Tuck the puff pastry down the sIdes of the pan.\nBake for 45 to 50 minutes, until golden brown.\nLet cool in the pan for one hour. Invert the Tarte Tatin onto a plate.\n",
					},
					new Recipe
					{
						Category = 5,
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/29/Chocolate_brownie_1.jpg/640px-Chocolate_brownie_1.jpg",
						Name = "Brownie",
						Description = "If you’ve been searching for the ultimate homemade brownie recipe, look no further. This is it!",
						Ingredients = "2 sticks (½ pound) unsalted butter\n8 ounces bittersweet or semisweet chocolate, roughly chopped (I use Ghirardelli bars)\n4 large eggs\n½ teaspoon salt\n1 cup granulated sugar\n1 cup firmly packed dark brown sugar\n2 teaspoons vanilla extract\n1 cup all-purpose flour, spooned into measuring cup and leveled off with knife ",
						Instructions = "\nSet the rack in the mIddle of the oven and preheat to 350°F. Line a 13 x 9 x 2-inch pan with parchment paper (bring parchment up sIdes of pan so there is a slight overhang) and grease with butter or nonstick cooking spray.\n\nPlace the butter in a medium microwave-safe bowl and melt in the microwave until bubbling. Add the chocolate and whisk until the chocolate is completely melted. The heat from the butter should be enough to melt the chocolate completely, but if not, place the chocolate-butter mixture in the microwave and heat for 20 seconds or so, then whisk again. (Alternatively, combine the butter and chocolate in a heat proof bowl and set over a pan of simmering water. Stir occasionally until melted.)\n\nWhisk the eggs in a large bowl. Add the salt, granulated sugar, brown sugar, and vanilla; whisk until smooth (be sure no lumps of brown sugar remain). Whisk in the chocolate-butter mixture, then add the flour and whisk until the batter is uniform.\n\nPour the batter into the prepared pan and spread evenly. Bake for about 45 minutes, until the top has formed a shiny crust and the batter is moderately firm. Cool completely in the pan on a rack. If not serving right away, store them at room temperature, for 3 to 4 days. To extend their shelf life for a day or two, you can refrigerate them.\n\nTo cut brownies, first lift them out of the pan using the parchment overhang and transfer them to a cutting board. Separate the parchment from the edges. Using a sharp knife, trim away the edges and cut the brownies into 2-in squares.",
					},
					new Recipe
					{
						Category = 1,
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/44/Mega_Muffin_-_Ready-%29_-_IMG_3667_%2813467371415%29.jpg/640px-Mega_Muffin_-_Ready-%29_-_IMG_3667_%2813467371415%29.jpg",
						Name = "Almond Flour Pumpkin Muffins",
						Description = "These almond flour pumpkin muffins are made with pumpkin puree, naturally sweetened with maple syrup, and contain no butter and no oil.",
						Ingredients = "1 1/2 cups blanched almond flour*\n½ teaspoon kosher salt\n3/4 teaspoon baking soda\n2 1/2 teaspoons ground cinnamon\n1/2 teaspoon ground cloves\n1/4 teaspoon ground nutmeg\n4 large eggs\n3/4 cup canned pumpkin (not pumpkin pie filling)\n1/3 cup pure maple syrup\n1 teaspoon pure vanilla extract\nUp to 1/2 cup mix-ins: chocolate chips cranberries, toasted and chopped walnuts or pecans, or a mix",
						Instructions = "Place a rack in the center of your oven, and preheat the oven to 350 degrees F. Line 10 of the wells of a standard 12-cup muffin pan with paper liners.\n\nIn a large bowl, stir together the almond flour, kosher salt, baking soda, cinnamon, cloves, and nutmeg. In a separate bowl, whisk together the eggs, pumpkin, maple syrup, and vanilla. Make a well in the center of the dry Ingredients, then pour in the wet. Gently stir, just until combined and the flour disappears. Fold in any desired mix-ins.\n\nDivIde the batter evenly between the cups, filling them nearly all the way to the top. Bake for 22 to 24 minutes, until a toothpick inserted in the center comes out clean. Place the muffin pan on a wire rack, and let cool in the pan for 5 minutes. Gently lift the muffins out of the pan, and place on the rack to finish cooling for as long as you can stand the suspense. Enjoy!\n\nNotes:\n*Be sure to use blanched almond flour, which is finely ground from blanched almonds that have the skin removed, not coarse almond flour (often called “meal”), which has the brown skins. No other flour can be substituted, as almond flour has very unique properties.\n\nTO MAKE ALMOND FLOUR: Place blanched, slivered almonds in a food processor and pulse until you have a fine powder. About 1 1/2 cups of slivered almonds will yield the 1 1/2 cups flour needed for the recipe. Be sure to measure before baking. Depending upon your food processor, you may also want to process the almonds in two batches to ensure they blend evenly.\n\nTO STORE: Keep leftovers in an airtight container lined with paper towels in the refrigerator for up to 5 days.",
					},
					new Recipe
					{
						Category = 1,
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c8/01_Pork_Burrito_-_The_Woods_Taco_Cart.jpg/640px-01_Pork_Burrito_-_The_Woods_Taco_Cart.jpg",
						Name = "Breakfast Burritos",
						Description = "Filled with sausage, eggs, cheese and fresh avocado salsa, these bodega-style breakfast burritos are delish any time of day!",
						Ingredients = "For the Avocado-Tomato Salsa\n    1 large avocado, peeled, pitted, and diced\n    ½ cup diced seeded tomatoes, from 1 to 2 tomatoes\n    1 small shallot, minced (about 2 tablespoons)\n    1 clove garlic, minced\n    1 jalapeño pepper, seeded and minced\n    1 tablespoon fresh lime juice, from 1 lime\n    ½ teaspoon salt\n    ¼ teaspoon ground cumin\n    ¼ cup fresh chopped cilantro\n\nFor the Burritos\n    4 large eggs\n    ¼ teaspoon smoked paprika\n    ¼ teaspoon salt\n    ½ lb spicy sausage (such as chorizo, Italian, or anything you like), removed from casings\n    1⅓ cups (6 oz) shredded Monterey Jack cheese\n    4 (10-in) burrito-size flour tortillas\n    Vegetable oil\n",
						Instructions = "Make the Avocado-Tomato Salsa: Place all of the Ingredients in a medium bowl and mix to combine. Set asIde.\n\nIn a medium bowl, whisk the eggs with the smoked paprika and salt. Set asIde.\n\nHeat a large nonstick pan over medium-high heat. Add the sausage and cook, stirring frequently, until browned, 4 to 5 minutes. Use a slotted spoon to transfer the sausage from the pan to a plate, leaving the drippings in the pan. Reduce the heat to low. Add the eggs and scramble until just cooked through. Transfer the eggs to a plate. Clean the pan (you'll use it again).\n\nAssemble the burritos: Spoon about ¼ cup of the avocado-salsa onto each tortilla (you'll have a little leftover salsa; that's for the cook!), followed by a quarter of the sausage, a quarter of the eggs, and ⅓ cup cheese. Fold in the sIdes of the tortilla over the filling and roll, tucking in the edges as you go.\n\nLightly coat the pan with oil and set over medium heat. When the pan is hot, add the burritos, seam sIde down. Cook, covered, until the bottom of the burritos are golden brown, about 3 minutes. Flip the burritos over and continue cooking, covered, until golden, a few minutes more. Serve warm.\n\nMake Ahead: The burritos may be assembled a few hours ahead of time, wrapped tightly in plastic wrap and refrigerated, before cooking. To reheat leftover burritos, wrap in foil and warm in a 350°F oven for about 15 minutes. (They won't be as crisp as they are fresh out of the pan, but they reheat well.)",
					},
					new Recipe
					{
						Category = 5,
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/0e/Biscotti_di_Prato_con_mandorle.jpg/640px-Biscotti_di_Prato_con_mandorle.jpg",
						Name = "Almond Biscotti",
						Description = "These almond biscotti are everything you want biscotti to be: buttery, lightly sweet, crunchy, and delicious any time of day!",
						Ingredients = "2½ cups all purpose flour, spooned into measuring cup and leveled off with knife\n¼ cup cornmeal or almond flour or almond meal\n1 teaspoon baking powder\n1 teaspoon salt\n1 teaspoon anise seeds, crushed with the back of a spoon into a powder\n10 tablespoons unsalted butter\n½ - ¾ cup sugar\n2 large eggs\n2 teaspoons vanilla extract\n½ teaspoon almond extract\n1¾ cups slivered almonds, chopped",
						Instructions = "Preheat the oven to 350°F and set the oven racks in the upper and mIddle thirds of the oven. Line two baking sheets with parchment paper.\n\nIn a medium bowl, whisk together the flour, cornmeal, baking powder, salt and crushed anise seeds.\n\nIn the bowl of an electric mixer, cream the butter and sugar until light and fluffy, about 2 minutes. Add the eggs, one at a time, beating well after each addition and scraping down the bowl as necessary. Mix in the vanilla and almond extracts. Add the flour mixture and almonds and mix on low speed until just combined. Dust your hands lightly with flour and divIde the dough into evenly into two disks; wrap in plastic wrap and refrigerate for at least 15 minutes.\n\nRemove the dough from the refrigerator and divIde each disk into two equal pieces. Dust your hands with flour and form each portion into logs about 2-inches wIde and ¾-inch tall directly on the lined baking sheets (if the dough is sticky, dust your hands with more flour as necessary). Leave about 4 inches of space between the logs to allow the dough to spread. Bake for 25-30 minutes, rotating the pans from top to bottom and front to back mIdway through, until the loaves are firm to the touch and golden around the bottom edges. Remove from the oven and let cool for 20 minutes.\n \nOnce cool, transfer the logs to a cutting board. Using a serrated knife and a sawing motion, cut the logs diagonally into generous ½-inch slices. (They will look a little undercooked in the mIddle.) Arrange the cookies, cut sIde down, back on one of the lined baking sheets. It will be a tight squeeze; it's not necessary to leave any space between the cookies. Return to the oven on the mIddle rack and cook for 5-7 minutes, until lightly golden on the undersIde. Remove the pan from the oven, carefully flip the biscotti over and cook for 5 minutes more, until lightly golden all over. Let cool on the baking sheet completely before serving. The cookies will keep in an airtight container for up to a month.\n\nFreezer-Friendly Instructions: The dough can be frozen for up to 3 months: Shape the dough into logs, wrap each securely in plastic wrap, and place them in a sealable bag. When ready to bake, remove the logs from the freezer, thaw the dough until pliable, and then proceed with recipe. To freeze after baking: After the cookies are completely cooled, double-wrap them securely with aluminum foil or plastic freezer wrap. Thaw overnight on the countertop before serving.",
					},
					new Recipe
					{
						Category = 5,
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/8/84/Chocolate_Cake_Flourless_%281%29.jpg/640px-Chocolate_Cake_Flourless_%281%29.jpg",
						Name = "Flourless Chocolate Almond Cake with Chocolate Ganache Frosting",
						Description = "Made with ground almonds and chocolate ganache, this flourless chocolate cake is rich and elegant.",
						Ingredients = "For the Cake\n    1½ cups slivered almonds\n    Handful fine dry breadcrumbs, matzo meal or gluten-free substitute (for dusting the pan)\n    6 ounces semisweet chocolate, finely chopped (best quality, such as Ghirardelli)\n    ¾ cup sugar, divIded\n    1½ sticks (6 ounces) unsalted butter, at room temperature, plus more for greasing the pan\n    6 large eggs\n    ⅛ teaspoon salt\n    1 teaspoon fresh lemon juice\n\nFor the Ganache Frosting\n    ½ cup heavy cream\n    2 teaspoons instant coffee or espresso powder\n    8 ounces semisweet chocolate, finely chopped (best quality, such as Ghirardelli)\n",
						Instructions = "Preheat the oven to 350°F. Line a baking sheet with parchment paper.\n\nSpread the almonds in a single layer on the prepared pan and bake for 5-7 minutes, or until the almonds are lightly colored and fragrant. Set asIde to cool. Leave the oven on.\n\nButter the bottom and sIdes of a 9'' x 3'' springform pan and line the bottom with a round of parchment paper cut to fit. Butter the paper. Dust the pan all over with the fine bread crumbs (or matzo meal); rotate the pan several times to spread evenly, then invert over the sink and tap lightly to shake out any excess crumbs. Set the prepared pan asIde.\n\nPlace the chocolate in a microwave-safe bowl. Microwave in 20-second intervals, stirring in between, until about 75% melted. Stir and let the resIdual heat melt the chocolate until completely smooth. Set asIde until tepId.\n\nPlace the almonds and ¼ cup of the sugar (reserve remaining ½ cup sugar) in a food processor fitted with a metal chopping blade. (Reserve the parchment paper from the nuts for icing the cake.) Process until the nuts are finely ground, stopping the machine once or twice to scrape down the sIdes. You should process for about one minute total. The mixture will be a little pasty but should not be the consistency of a nut butter. Set asIde the ground nuts.\n\nIn the large bowl of an electric mixer, beat the butter until soft. Add ¼ cup of the sugar (reserve the remaining ¼ cup sugar) and beat to mix. Add the egg yolks, one at a time, beating and scraping the sIdes of the bowl as necessary until smooth. On low speed, add the chocolate and beat until mixed. Then add the processed almonds and beat, scraping the bowl, until incorporated.\n\nNow, the whites should be beaten in the large bowl of the mixer. If you don't have an additional large bowl for the mixer, transfer the chocolate mixture to any other large bowl. Wash the bowl and beaters.\n\nIn the clean bowl of the mixer, with clean beaters, beat the egg whites with the salt and lemon juice, starting on low speed and increasing it gradually. When the whites barely hold a soft shape, gradually add the remaining ¼ cup sugar. Continue to beat until the whites hold stiff peaks when the beaters are raised. Do not overbeat.\n\nStir a large spoonful of the whites into the chocolate mixture to lighten it a bit. Then, in three additions, fold in the remaining whites. Do not fold thoroughly until the last addition and do not handle more than necessary.\n\nTurn the mixture into the prepared pan. Rotate the pan briskly in order to level the batter.\n \nBake for 20 minutes at 350°F, then reduce the temperature to 325°F and continue to bake for an additional 50 minutes. The top might crack a bit; that's okay.\n\nRemove the cake pan from the oven and place it on a rack. Let stand until tepId, 50 to 60 minutes.\n\nRelease and remove the sIdes of the pan (do not cut around the sIdes with a knife—it will make the rim of the cake messy). Now, let the cake stand until it is completely cool.\n\nThe cake will sink a little in the mIddle as it cools. Use a long, thin, sharp knife (I prefer serrated) and level the top. It will seem like you're cutting off a lot; don't worry about it. The finished cake should be about 1½-inches high. Brush away any loose crumbs. Place a rack or a small board over the cake and carefully invert. Remove the bottom of the pan and the paper lining.\n\nThe cake is now upsIde down; this is the way it will be iced. Place 4 strips of the reserved parchment paper (each about 3'' x 12'') around the edges of a cake plate. With a large, wIde spatula, carefully transfer the cake to the plate; check to be sure that the cake is touching the paper all around (in order to keep the icing off the plate when you ice the cake).\n\nTo make the ganache frosting, heat the cream in a medium saucepan over medium heat until it boils. Add the espresso or coffee powder and whisk to dissolve. Add the chocolate and remove from the heat. Stir until all the chocolate is all melted and the mixture is smooth. Let the ganache stand at room temperature, stirring occasionally, for about 15 minutes, or until it begins to thicken.\n\nPour the ganache slowly over the top of the cake. Using a long, narrow metal spatula, smooth the top and spread the icing so that a little runs down the sIdes of the cake (not too much—the icing on the sIdes should be a much thinner layer than on the top). Smooth the sIdes with the spatula, then remove the parchment liners.",
					},
					new Recipe
					{
						Category = 1,
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/aa/Blueberry_pancakes_%281%29.jpg/640px-Blueberry_pancakes_%281%29.jpg",
						Name = "Pancakes",
						Description = "Rise and shine to the fluffiest and crispiest pancakes you’ve ever tasted with this family-favorite homemade pancake recipe.",
						Ingredients = "2 cups all-purpose flour, spooned into measuring cup and leveled off\n1 tablespoon baking powder\n¼ cup sugar\n1 teaspoon salt\n2 large eggs\n1½ cups milk, plus more if necessary\n4 tablespoons unsalted butter, melted and slightly cooled, plus more for cooking\nVegetable oil, for cooking",
						Instructions = "\nIn a large bowl, whisk together the flour, baking powder, sugar and salt.\n\nIn a medium bowl, whisk the eggs and milk until evenly combined.\n\nPour the milk/egg mixture and the melted butter into the dry Ingredients and whisk until just combined. If the batter seems too thick, add 1 to 2 tablespoons more milk.\n\nHeat a grIddle or nonstick pan over medium heat; coat it lightly with vegetable oil and swirl in a thin pat of butter. Ladle or drop the batter onto the grIddle, using approximately ¼ cup for each pancake; cook until the first sIde is golden brown, or until the top surface bubbles and is dotted with holes. Flip and cook until the other sIde is golden brown. This happens quickly so peek after 30 seconds and watch carefully! Adjust the heat setting if necessary. Wipe the grIddle clean with a paper towel between batches. Serve immediately with maple syrup.\n\nFreezer-Friendly Instructions: The pancakes can be frozen for up to 3 months. After they are completely cooled, place a sheet of parchment or wax paper between each pancake and stack together. Wrap the stack of pancakes tightly in aluminum foil or place insIde a heavy-duty freezer bag. To reheat, place them in a single layer on a baking sheet and cover with foil. Bake in a 375°F oven for about 8 to 10 minutes, or until hot.",
					},
					new Recipe
					{
						Category = 1,
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/7/72/German_pancake_in_Portland.jpg/640px-German_pancake_in_Portland.jpg",
						Name = "Dutch Baby",
						Description = "Start your day off with a Dutch baby, a big, puffy pancake baked in a sizzling-hot buttered skillet.",
						Ingredients = "3 large eggs\n½ cup all-purpose flour, spooned into measuring cup and leveled-off\n½ cup milk\n1 tablespoon maple syrup, plus more for serving\n¼ teaspoon salt\n½ teaspoon vanilla extract\n3 tablespoons unsalted butter\nConfectioners' sugar, for serving (optional)\nFresh berries, for serving (optional)",
						Instructions = "\nPreheat oven to 400°F and set an oven rack in the mIddle position. Put a 10-inch cast iron skillet or oven-safe nonstick pan into the oven and heat for at least 5 minutes.\n\nIn a blender, combine the eggs, flour, milk, 1 tablespoon maple syrup, the salt, and vanilla. Blend until completely smooth, scraping down the sIdes of the blender jar as necessary, about 30 seconds.\n \nOpen the oven door and drop the butter into the preheated skillet. Close the oven and allow the butter to melt, about 2 minutes (do not let it burn). Carefully remove the hot skillet from the oven and place an oven mitt or dishtowel over the handle to remind yourself that it's hot. Pour the batter into the buttered skillet and carefully place the skillet back into the oven. Bake for about 20 minutes, until puffed and golden. Carefully remove the skillet from the oven (again, place an oven mitt or dishtowel over the handle to remind yourself that it's hot). Dust with confectioners' sugar and top with berries, if desired, then cut into wedges and serve with maple syrup.",
					},
					new Recipe
					{
						Category = 3,
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f1/Kung_Pao_Chicken_%40_Rice_%26_Noodle_%40_Montparnasse_%40_Paris_%2833647529885%29.jpg/640px-Kung_Pao_Chicken_%40_Rice_%26_Noodle_%40_Montparnasse_%40_Paris_%2833647529885%29.jpg",
						Name = "Kung Pao Chicken",
						Description = "Kung Pao chicken, a classic Chinese takeout dish of stir-fried chicken, peanuts, and vegetables, is easy to make at home. ",
						Ingredients = "For the Marinade\n    1½ tablespoons soy sauce\n    1 tablespoon dry sherry\n    2 teaspoons corn starch\n    1½ lb chicken tenderloins, cut into 1-in pieces\n\nFor the Sauce\n    1 tablespoon balsamic vinegar\n    2 tablespoons soy sauce\n    1 tablespoon hoisin sauce, best quality such as Kikkoman or Lee Kum Kee\n    1 tablespoon Asian/toasted sesame oil\n    1½ tablespoons sugar\n    1 tablespoon corn starch\n    ½ teaspoon crushed red pepper flakes (use half the amount for a milder sauce)\n    ¼ teaspoon ground ginger\n    ⅓ cup water\n\nFor the Stir-fry\n    2½ tablespoons vegetable oil\n    1 large red bell pepper, diced\n    2 stalks celery, halved lengthwise and thinly sliced\n    ¼ teaspoon salt\n    3 cloves garlic, chopped\n    5 scallions, white and green parts, thinly sliced\n    ⅓ cup whole roasted unsalted peanuts or cashews\n",
						Instructions = "Marinate the chicken: In a medium bowl, whisk together the soy sauce, dry sherry, and cornstarch until the cornstarch is dissolved. Add the chicken and toss to coat. Let stand at room temperature for 15 minutes, stirring occasionally.\n\nPrepare the sauce: In another medium bowl, whisk together all of the sauce Ingredients until the cornstarch is dissolved (it can stick to the bottom of the bowl so be sure to scrape it up).\n \nHeat a large nonstick skillet over high heat until very hot. Add 1 tablespoon of the oil and swirl to coat. Add the bell pepper, celery, and salt and cook, stirring frequently, until slightly softened and starting to brown, about 5 minutes. Transfer the vegetables to a large bowl and set asIde.\n\nAdd an additional ½ tablespoon of oil to the pan and set over high heat. Add half of the chicken (it's important not to crowd the pan) and brown on one sIde, about 1½ minutes. Turn the chicken pieces and continue cooking for about 1½ minutes more, or until the chicken is just cooked through. Transfer the chicken to the bowl with the peppers and celery. Add another ½ tablespoon of oil to the pan. Add the remaining chicken and cook until golden on one sIde, about 1½ minutes. Turn the chicken pieces over and cook for 1 minute. Add ½ tablespoon more oil to the pan, along with the garlic and scallions, and cook, stirring with the chicken, for about 30 seconds more.\n \nAdd the reserved vegetables and reserved chicken to the pan, along with the sauce. Reduce the heat to low and cook until the chicken and vegetables are warmed through and the sauce is thickened, about 30 seconds. Stir in the nuts. Taste and adjust seasoning, if necessary, and serve. (Note: the sauce will thicken as it sits; thin it with a few tablespoons of water, if necessary.)",
					},
					new Recipe
					{
						Category = 4,
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d5/KaleSalad-0348.jpg/640px-KaleSalad-0348.jpg",
						Name = "Kale Salad with Ginger Peanut Dressing",
						Description = "A kale salad you’ll actually crave.",
						Ingredients = "For the Salad\n    4 cups chopped curly kale, thick stems removed (patted dry)\n    3 cups shredded red cabbage\n    2 cups shredded carrots\n    1 red bell pepper, sliced into bite-sized pieces\n    ¾ cup slivered almonds\n    ½ cup chopped fresh cilantro\n\nFor the Dressing\n    3 tablespoons creamy peanut butter\n    3 tablespoons unseasoned rice vinegar\n    1 tablespoon fresh lime juice, from one lime\n    3 tablespoons vegetable oil\n    1 tablespoon soy sauce\n    3 tablespoons honey\n    1 tablespoon sugar\n    1 large clove garlic, roughly chopped\n    1-inch square piece fresh ginger, peeled and roughly chopped (see note)\n    ¾ teaspoon salt\n    1 teaspoon sriracha\n    1 teaspoon toasted sesame oil",
						Instructions = "Preheat the oven to 350°F. Line a baking sheet with parchment paper or aluminum foil.\n\nBake the almonds until lightly golden and fragrant, 5-10 minutes. (Keep a close eye on them; nuts burn quickly.) Transfer the nuts to a small plate to cool.\n\nMeanwhile, combine all of the Ingredients for the dressing in a food processor or blender; process until smooth and creamy.\n\nCombine the Ingredients for the salad in a large mixing bowl. Pour the dressing over the salad and toss well. Let the salad sit at room temperature for 15 minutes to allow the kale to soften. Toss again, and then taste and adjust seasoning, if necessary. (As the salad sits, the flavors may dull; a squeeze of lime juice will wake things up.)",
					},
					new Recipe
					{
						Category = 4,
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6f/Brussels_Sprouts_%26_Grapefruit_Salad_with_Mint_%26_Red_Onions_%288440148972%29.jpg/640px-Brussels_Sprouts_%26_Grapefruit_Salad_with_Mint_%26_Red_Onions_%288440148972%29.jpg",
						Name = "Shaved Brussels Sprout Salad with Apples, Walnuts & Parmesan",
						Description = "This sweet and tangy Brussels sprout salad is best prepared ahead of time.",
						Ingredients = "1½ pounds Brussels sprouts, shredded\n1 large tart-sweet red apple, such as Honey Crisp, cored and chopped (no need to peel)\n3 tablespoons minced shallots, from 1 large shallot\n¼ cup extra-virgin olive oil\n¼ cup vegetable oil\n¼ cup + 2 tablespoons apple cIder vinegar\n3 tablespoons honey\n1 teaspoon salt\n¼ teaspoon freshly ground black pepper\n¾ cup walnuts, toasted if desired (see note) and coarsely chopped\n¾ cup thinly sliced and crumbled Parmigiano-Reggiano",
						Instructions = "Preheat the oven to 350°F and set an oven rack in the mIddle position.\n\nPlace the walnuts in a single layer on a baking sheet. Bake, checking frequently, until lightly toasted and fragrant, 6 to 10 minutes. Transfer immediately to a plate and let cool.\n\nIn a large bowl, combine the shredded Brussels sprouts, apples, shallots, olive oil, vegetable oil, apple cIder vinegar, honey, salt, and pepper; toss well. Cover with plastic wrap and refrigerate for at least 30 minutes and up to 4 hours to allow the sprouts to soften and the flavors to marry.\n\nWhen ready to serve, toss the walnuts and Parmigiano-Reggiano with the salad. Taste and adjust seasoning if necessary, then serve. Leftovers keep well in the refrigerator for 1 or 2 days but keep in mind that the flavors will dull the longer the salad sits; add a little more cIder vinegar and vegetable oil to perk it up.",
					},
					new Recipe
					{
						Category = 3,
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/3/30/Coq_au_vin_rouge.jpg/640px-Coq_au_vin_rouge.jpg",
						Name = "Coq au Vin",
						Description = "Coq au vin is a hearty French stew of chicken braised in red wine with mushrooms and crisp pancetta.",
						Ingredients = "3 tablespoons olive oil, divIded\n4 ounces diced pancetta (or bacon)\n8 bone-in, skin-on chicken thighs (about 4 pounds), trimmed of excess skin (see note)\nSalt\nFreshly ground black pepper\n1 large yellow onion, roughly chopped\n4 cloves garlic, roughly chopped\n¼ cup Cognac\n2½ cups red wine, preferably Burgundy or Pinot Noir\n2½ cups chicken broth\n1½ tablespoons tomato paste\n2 teaspoons balsamic vinegar\n1½ teaspoons sugar\n1 tablespoon fresh thyme leaves (or 1 teaspoon dried)\n1 bay leaf\n3 large carrots, peeled and cut into ½-inch chunks on the bias\n8 ounces sliced cremini mushrooms\n4 tablespoons unsalted butter, softened\n4 tablespoons all-purpose flour ",
						Instructions = "Heat 1 tablespoon of the oil in a large (5-qt) Dutch oven or heavy-bottomed pot over medium heat. Add the pancetta and cook until the fat has rendered and the pancetta is crispy, 5 to 8 minutes. Using a slotted spoon, transfer the pancetta to a paper-towel-lined plate, leaving the fat in the pan.\n\nSeason the chicken all over with 2 teaspoons salt and ½ teaspoon pepper. Increase the heat to medium-high and brown half of the chicken in a single layer, skin sIde down, until golden and crispy, about 5 minutes (brown on the skin sIde only). Using tongs, transfer the chicken to a plate; set asIde. Repeat with the remaining chicken. Pour off all but about 2 tablespoons of the fat.\n\nReturn the pot to the stove and reduce the heat to medium-low. Add the onions to the pot and cook, stirring occasionally, until the onions are softened and just starting to brown, 3 to 5 minutes. Add the garlic and cook, stirring constantly, until fragrant, about 1 minute more. Add the Cognac and cook, stirring to scrape the brown bits from the bottom of the pan, until the Cognac has evaporated. Add the wine, chicken broth, tomato paste, balsamic vinegar, sugar, thyme, bay leaf, and ½ teaspoon salt. Bring to a boil, then reduce the heat to medium and gently boil, uncovered, for 15 minutes.\nAdd the chicken and any accumulated juices from the plate back to the pot, along with the carrots. Bring to a simmer, then cover and cook over low heat for 30 minutes, or until the chicken and carrots are cooked through.\n\nWhile the chicken cooks, heat the remaining 2 tablespoons of oil in a large skillet over medium heat. Add the mushrooms and ¼ teaspoon salt and cook, stirring frequently, until the mushrooms are golden brown, about 5 minutes. Set asIde.\n\nAlso while the chicken cooks: In a small bowl, mash the softened butter and flour to make a smooth paste. Set asIde.\n\nUsing a slotted spoon, transfer the cooked chicken to a plate.\n\nIncrease the heat in the Dutch oven/pot to medium and stir in three-quarters of the flour and butter paste. Gently boil until the sauce is thickened, 5 to 7 minutes; add the remaining paste if you'd like the sauce a little thicker. Fish out and discard the bay leaf.\n\nUsing a fork and knife, pull the skin off of the chicken and discard.\nAdd the chicken and any accumulated juices back to the pot and simmer, uncovered, for about 10 minutes. Right before serving, stir in the browned mushrooms and pancetta. Taste and adjust seasoning, if necessary, then serve.\n\nNote: Sometimes chicken thighs have excess skin and/or fat. Before cooking, using kitchen shears, trim any skin that extends farther than the edges of the chicken thigh, and snip off any excess fat.\n\nMake-Ahead Instructions: Let cool to room temperature and then store in the refrigerator for up to 2 days. Reheat over medium-low heat on the stovetop before serving. (For best results, store the sautéed mushrooms and crispy pancetta in separate containers in the refrigerator and add before serving.)\nFreezer-Friendly Instructions: This can be frozen for up to 3 months. Before serving, defrost the stew in the refrigerator for 24 hours and then reheat on the stovetop over medium-low heat until hot.",
					},
					new Recipe
					{
						Category = 1,
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c6/Souffe_pancakes_%2876651%29.jpg/640px-Souffe_pancakes_%2876651%29.jpg",
						Name = "Fluffy Japanese Soufflé Pancakes",
						Description = "These pancakes are like eating cottony clouds, but even better with homemade whipped cream and fresh berries.",
						Ingredients = "2 large eggs (50 g each w/o shell)\n1½ Tbsp whole milk (I haven‘t tried reduced-fat, low-fat, nonfat, or plant-based milk for this recipe as I believe the batter will be too thin to make souffle pancakes)\n¼ tsp pure vanilla extract\n¼ cup cake flour (weigh your flour or use the “fluff and sprinkle“ method and level it off; you can make homemade cake flour)\n½ tsp baking powder\n2 Tbsp sugar\n1 Tbsp neutral oil (for greasing the pan)\n2 Tbsp water (for steaming)\n\nFor the Fresh Whipped Cream (optional)\n    ½ cup heavy (whipping) cream\n    1½ Tbsp sugar (add more if you like it sweeter)\n\nFor the Toppings\n    1 Tbsp confectioners’ sugar\n    fresh berries (strawberries, blueberries, etc.)\n    maple syrup",
						Instructions = "To Mix the Batter\nSeparate 2 large eggs (50 g each w/o shell) into whites and yolks in two different bowls. Put the bowl with the egg whites in the freezer for 15 minutes. \nIn the meantime, add 1½ Tbsp whole milk and ¼ tsp pure vanilla extract to the egg yolks and whisk using a hand whisk until thick and frothy.\nSift ¼ cup cake flour and ½ tsp baking powder into the bowl.\nWhisk to combine thoroughly; do not overmix. Set asIde while you make the meringue.\n\nTo Make the Meringue\nAfter 15 minutes, take out the bowl with the egg whites from the freezer. The egg whites should be half frozen. Now, start beating the egg whites with a hand mixer (you can also use a stand mixer or balloon whisk).\nWhen the egg whites turn frothy and opaque, gradually add in 2 Tbsp sugar, roughly one-third of it at a time. Then, increase the mixer speed to high (Speed 10) and beat vigorously until stiff peaks form (see the next step for how to check). It takes about 2 minutes of beating at high speed to reach stiff peaks. Tip: When using a stand mixer, I usually pause beating when the meringue is almost done. Take off the whisk attachment from the mixer and use it to hand-mix the looser egg whites near the bowl's edge into the stiffer whites near the center until it's all homogenous in texture. Then, put the whisk back on and continue beating.\nTo check for stiff peaks, stop whisking and pull up your beaters or whisk. The meringue in the bowl or on the whisk should be firm enough to hold a peak, pointing straight up (or maybe folding over a little bit just at the very tips). By this time, the meringue should have a glossy texture, too. Tip: If you overbeat the meringue, it will become very stiff and grainy and won't incorporate into the batter at all.\n\nHeat a large nonstick frying pan to 300ºF (150ºC) over the lowest heat. Brush with 1 Tbsp neutral oil and lightly remove any visible oil with a paper towel (otherwise the pancakes will have a spotty pattern). Keep the pan on low heat while you fold in the egg white meringue into the egg yolk mixture in the next step.\n\nTo Fold In the Meringue\nTake one-third of the egg white meringue and add to the egg yolk mixture. Whisk together by hand (don’t worry too much about breaking air bubbles at this point).\nNext, take half of the remaining meringue and add to the egg yolk mixture. Using a hand whisk, gently fold them in without breaking the air bubbles in the egg whites.\nNow, transfer the egg yolk mixture back into the bowl with the remaining meringue. Very gently fold the two mixtures together, taking care not to deflate the air bubbles in the meringue and batter as you fold. Mix the batter very gently until well combined and homogenous.\n\nTo Cook the Pancakes\nKeep your nonstick frying pan heated to 300ºF (150ºC) at all times over low heat. Remember, each pancake gets roughly four small scoops of batter, and you will be making three pancakes. For the first pancake, place one scoop of batter and make a tall mound in the frying pan, using a small ladle or a serving spoon (that’s bigger than a regular spoon—probably 2–3 Tbsp). Next, stack one more scoop of batter onto the first scoop already in the pan. Repeat for the next two pancakes, giving each pancake two scoops of batter.\nBy the time all three pancakes have two scoops, the surface of the batter is slightly dry already. At this point, you can mound one more scoop on top of each pancake, keeping the batter piled up high. In the bowl, you should still have roughly three scoops left (if you have slightly more, that’s okay).\nSet the timer for 6–7 minutes and add 1 Tbsp of the 2 Tbsp water in three empty spaces insIde the pan. Cover with a lId. The steam from the water keeps the pancakes moist while they cook. Please note: The suggested time is just a guIdeline; how long you will cook the pancakes is based on the temperature of your frying pan.\nAfter 2 minutes have passed, open the lId, and add one final scoop of batter to each pancake (or more scoops if you have more batter). Make sure to stack the batter high, not wIde. If the water has evaporated, add a little bit more. Cover with the lId and cook.\nAfter 6–7 minutes have passed, lift the pancake VERY GENTLY using an offset spatula. If the pancake is stuck, don’t touch it until it firms up a little. If you force it, the pancake will crack in the mIddle. When the pancake is ready, you can easily move the pancake. Repeat with the other pancakes.\nHere is another set of Images to show the process. Slightly pull the pancake to create an empty space and gently flip it over with a “rolling over” motion.\nAdd another 1 Tbsp water to the empty spaces in the pan and cover. Set the timer for 4–5 minutes to cook the other sIde on the lowest heat setting.\nOnce they are nicely browned, transfer the pancakes to your serving plates. \n\nPlace the optional fresh whipped cream on the pancakes and top with fresh berries. Dust your Fluffy Japanese Soufflé Pancakes with 1 Tbsp confectioners’ sugar and drizzle with maple syrup. Enjoy!\n\nNotes\nHow to Prevent Your Soufflé Pancakes from Collapsing:\nSoufflé pancakes can be tricky to make (probably not easy for a beginner cook), so make sure to read my tips in the post thoroughly before you start cooking.\nBeat your egg whites correctly. Underbeating or overbeating will cause the pancakes to deflate after cooking.\nCook over low heat, and make sure the insIdes of the pancakes are properly cooked through. If the insIde is not cooked through, there is no structure to hold up the pancakes and they will collapse as soon as the temperature drops.\n",
					},
					new Recipe
					{
						Category = 5,
						Image = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/e8/Taza_Chocolate_Torte_%286469995731%29.jpg/640px-Taza_Chocolate_Torte_%286469995731%29.jpg",
						Name = "Chocolate Truffle Torte",
						Description = "This flourless chocolate torte is a better and purer vehicle for chocolate than chocolate itself.",
						Ingredients = "1 pound (454 grams) bittersweet chocolate (fine quality that you love eating, no higher than 62%)\n1/2 pound (2 sticks, or 227 grams) unsalted butter, room temperature\n6 large eggs (300 grams, out of the shell), room temperature if possible",
						Instructions = "Butter and bottom line with buttered parchment 8-inch spring form pan at least 2 1/2 inches high, wrap the outsIde of pan with a double layer of heavy-duty foil. \n \nHeat the oven to 425°F.\n\nIn a large heat-proof bowl set over a pan of hot, not simmering water (do not allow the bottom of the bowl to touch the water) place the chocolate and butter and allow it to stand, stirring occasionally, until smooth and melted. (You can also use a microwave on higher power, stirring every 20 seconds.)\n\nIn a large mixer bowl, set over a pan of simmering water, heat the eggs, stirring constantly with a wire whisk, until just warm to the touch. Immediately remove the bowl to the stand mixer and with the whisk attachment on high speed, beat about 5 minutes, until triple in volume and the eggs are billowy and lighter in color. (If using a handheld electric mixer, beat the eggs over simmering water until they are hot. Then remove them from the heat and beat for about 5 minutes or until cold.)\n\nUse as large wire whisk or rubber spatula to fold half the eggs into the chocolate mixture until almost evenly incorporated. Fold in the remaining eggs until almost no streaks remain. Use a rubber spatula to finish folding, scraping up the mixture from the bottom to ensure that all the heavier chocolate mixture gets incorporated.\n\nScrape the mixture into the prepared pan and set it in the larger pan (10-inch cake pan or roasting pan). Place it in the oven and surround it with 1 inch of hot water. Bake for 5 minutes. Cover it loosely with a sheet of buttered foil and bake another 10 minutes. (It will wobble when moved.) Remove the cake pan from the water bath and allow it to cool for about 45 minutes. Cover tightly with plastic wrap and refrigerate it until very firm, at least 3 hours.\n \nUnmold the cake: Have ready a serving plate that has at least an 8-inch flat center portion and an 8-inch or large flat loose bottom of a tart pan or plate, covered with greased plastic wrap.\n\nUse a torch, hair drier, or a hot damp towel to wipe the sIdes of the pan.\n\nRun a thin metal spatula around the sIdes of the torte and release the sIdes of the springform pan. Place the plastic-wrapped plate on top and invert the torte onto it. Heat the bottom of the pan and remove it. Peel off the parchment and reinvert the torte onto the serving plate.\n\nServe: It is most moussey and delicious at room temperature. Cut the torte, using a thin-bladed knife dipped in hot water between each slice. Accompany with raspberry sauce and fresh raspberries and whipped cream if desired.",
					},
					new Recipe
					{
						Category = 5,
						Image = "",
						Name = "Olive Oil Cake with Grapes and Fennel",
						Description = "Grapes, fennel, and olive oil star in this simple but unexpected cake. The batter comes together in one bowl and is dotted with ripe, juicy grapes. The cake is scattered with a mixture of sugar, fennel seeds, and olive oil that bakes into a crispy, fragrant, streusel-like topping. Serve the cake warm with a scoop of vanilla ice cream for an elegant dessert, but make sure to save some leftovers, because this cake is just as good for breakfast.",
						Ingredients = "For the cake: \n1 c (225g) extra-virgin olive oil\n1 c (200g) granulated sugar\n2 tsp baking powder\n1.5 tsp Diamond Crystal kosher salt\n3 large eggs\n1 c (240g) sour cream\n2 c (250g) all-purpose flour plus 2 teaspoons, divIded\n1.5 c (225–250g) grapes, such as Thomcord—larger varieties should be halved, smaller can be left whole, avoId varieties with a lot of seeds \n\nFor the fennel sugar:\n1 tsp dried fennel seeds\n1.5 tsp extra-virgin olive oil\n⅓ c (66g) granulated sugar\nPinch of salt ",
						Instructions = "Heat the oven to 325°F. Prepare a 9-inch springform pan by greasing it and lining with parchment paper.     \n\nMake the fennel sugar topping. Use a mortar and pestle or spice grinder to grind the fennel down to a coarse powder. In a small mixing bowl, use your hands to rub the ground fennel into the sugar and salt until the mixture is fragrant. Add in the 1½ teaspoons of olive oil and stir with a spoon until the mixture takes on the texture of wet sand. Set asIde.  \n\nMake the cake. Toss half the grapes with 2 teaspoons of flour and set asIde. In a large mixing bowl, combine the olive oil, sugar, salt, baking powder, and eggs and whisk until well combined—the mixture should be thick and glossy. Add the sour cream and whisk until homogenous. Then add the flour and whisk until just combined before gently mixing in the floured grapes. Pour the batter into the prepared pan and place the remaining, un-floured grapes on top. Evenly distribute the fennel sugar over the batter. Bake for about 1 hour, or until golden brown and a toothpick inserted into the mIddle of the cake comes out clean. Let cool for 10 to 15 minutes before removing the cake from the pan. Serve warm or at room temperature. \n\nNote: Store leftover cake in an airtight container at room temperature for up to 3 days.               ",
					},
					new Recipe
					{
						Category = 3,
						Name = "Calzone",
						Instructions = "https://www.youtube.com/watch?v=6-jU9p3oEqA  --- This fun vIdeo starts with the details on how to master a great calzone or pizza dough, including what flour and yeast makes for best texture.  Then onto essential rolling techniques and you should never use a rolling pin. This Neapolitan classic if assembled with great fillings like, ricotta, sautéed spinach, mozzarella, salami and then topped again with tomato and cheese or left plain.  Do you like it Neapolitan or New York style?  Check out the difference.  \n\nBefore you start:  If you want to make it the same day, allow 3 hours of dough prep time. \nFor maximum flavour and dough structure. Make the dough in the evening, let rise  for 1 .5 hours, divIde, shape and keep in frIdge until next day.  \n\nFor baking, use a bread stone or steel, cast iron ( like mine ) in lower rack of your oven.  Heat preheat oven to 470 D F for 45 minutes so thoroughly heat.  \n\nDough:\n\nNote: if using dry active yeast, add to water with a little sugar, until foaming, and increase to ¾ tsp.  \n\nCombine flour with instant yeast, in a stand mixer to hook attachment.  Blend with spatula first to distribute yeast. Add the water and oil to the flour mixture.  Work on low speed and once flour has been absorbed, add the salt.  Turn to medium speed and work for about 4-5 minutes.  Dough will be sticky . Stop and let rest for 3-5 minutes.  Begin to work machine again , just to bring all dough together. \n\nTurn out onto clean surface  and continue kneading.  Resist the urge to add more flour if the dough is too sticky.  Let it rest again for a few minutes and quickly knead until smooth but still tacky to touch.  Use a bit of flour on hands if it’s too sticky to shape.  Dough should be completely smooth. \n\nPlace in a well oiled bowl and cover with plastic wrap to rise in warm spot until doubled in bulk, about 1 1/2 hours.  DivIde into 6 equal balls or 3 for larger ones.  Shape well into balls and place into oiled container well covered with space for dough to expand.  Place in frIdge and let proof for 3  hours or overnight for more flavour and improved texture.  \n\nAn hour before baking and when preparing your fillings , preheat stone or cast iron grIdle, in oven at 470 D. \nRemove dough from frIdge. \n In a medium skillet, heat olive oil and saute garlic on medium setting.  Add the spinach and toss a few times, seasoning with salt and pepper.  Cool and set asIde. \n\nOn a lightly floured surface, using your hands, press out dough, leaving centre a bit thicker than edges.  Drape over fists and use a gentle pull and stretch technique until you have an even round.  Press down edges. .   . DivIde filling between 6 rounds and mound on bottom half of circle leaving a 1/2 “ rim.  Fold top over sealing down with a fork or as per vIdeo. \n\nTear holes in top of calzone for steam to escape.  Sprinkle a baking sheet or pizza peal with semolina and transfer calzone gently on it and move to ensure it can slIde.  SlIde onto heated stone and bake at 470 D for 8-10 minutes. \n\nMakes 6  - small or 3 large \n",
						Ingredients = "CALZONE 2 WAYS\n\nDough: NOTE  metric measurements in dough are in weight \n2 cups 00 Pizza flour , unbleached (500 ml) 300 gm by weight – all purpose can be subbed for 00 pizza flour   \n½ tsp.  instant yeast (2 gm ) \n1 cup minus a Tbsp. Cool water  (200 ml)\n2 Tbsp. extra virgin olive oil (25 ml )\n1 Tsp. salt (5 g) \n\nFilling:\n1 small bunch spinach , blanched , squeezed of excess water \n1 Tbsp. olive oil (15 ml) \n1 clove garlic , chopped \n1 red pepper, roasted and diced \n2 oz. Cacciatore salami, sliced into small pieces (60 gm )\n2 Tbsp. olive oil (25 ml)\n4 oz.  sliced buffalo mozzarella (125 gm )\n½ cup shredded Pecorino cheese (125 ml)\n3/4  cup soft, drained ricotta cheese (100 gm ) \nChili flakes , optional \nsalt and pepper to taste\npesto, optional or fresh basil ",
					},
					new Recipe
					{
						Category = 5,
						Name = "Raspberry Almond Ricotta Cake",
						Description = "Moist ricotta cake dotted with fresh raspberries and topped with sliced almonds and confectioner’s sugar. This elegant cake is easy to make and can be served for breakfast, brunch, or dessert.",
						Ingredients = "1 1/2 cups all-purpose flour\n1 cup granulated sugar\n2 teaspoons baking powder\n1/2 teaspoon salt\n1 1/2 cups whole milk ricotta cheese\n3 large eggs, at room temperature\n1 1/2 teaspoons pure vanilla extract\n3/4 teaspoon almond extract\n1/2 cup unsalted butter, melted and slightly cooled\n1 1/2 cups raspberries, fresh or frozen\n2 tablespoons turbinado sugar\n1/2 cup sliced almonds\nConfectioner’s sugar, for dusting, optional",
						Instructions = "Preheat oven to 350 degrees F.\nIn a large bowl, whisk together the flour, sugar, baking powder, and salt.\nIn a medium bowl, whisk together the ricotta cheese, eggs, vanilla, and almond extract. Gradually add in the melted butter, whisking until combined and smooth.\nAdd the ricotta mixture to the dry Ingredients and stir with a spatula until just combined. Don’t over mix. Gently fold in the raspberries, being careful not to over mix or the batter will turn pink. It’s ok if you get a few streaks of pink.\nGrease a 9-inch springform pan generously with nonstick cooking spray. Scrape the batter evenly into the pan and sprinkle the top with turbinado sugar and sliced almonds.\nBake for 45 to 55 minutes or until golden brown and a toothpick inserted into the center of the cake comes out clean. Remove the pan from the oven and place on a cooling rack. Let the cake cool in the pan for 30 minutes, then carefully remove the sIdes of the springform pan.\nDust the cake with confectioner’s sugar, if desired. Cut the cake into slices and serve!",
					},
					new Recipe
					{
						Category = 5,
						Name = "Blueberry-Lemon Tiramisù",
						Description = "Layered with Ladyfingers dipped in a tangy lemon soak and a berry sauce swirled throughout the whipped, creamy layer, this Blueberry-Lemon Tiramisù is the light and airy treat that will take your summer celebrations to the next level.",
						Ingredients = "Blueberry Compote\n    Makes about 1 ¼ cups\n    9 ounces (256 grams) fresh blueberries (about 2 cups)\n    ⅔ cup (133 grams) granulated sugar\n    1 tablespoon (15 grams) fresh lemon juice\n    1 tablespoon (15 grams) water\n    ½ teaspoon (2 grams) vanilla extract\n    ¼ teaspoon kosher salt\n\nLadyfingers\n    Makes about 60 cookies\n    ⅔ cup (83 grams) unbleached cake flour\n    2 ½ tablespoons (20 grams) cornstarch\n    4 large eggs (200 grams), separated and room temperature\n    ½ cup (100 grams) plus 2 tablespoons (24 grams) granulated sugar, divIded\n    ½ teaspoon (2 grams) vanilla extract\n    2 tablespoons (14 grams) confectioners’ sugar\n\nBlueberry-Lemon Tiramisù\n    2 cups (480 grams) cold heavy whipping cream\n    1 tablespoon (3 grams) lemon zest\n    2 teaspoons (8 grams) vanilla extract\n    ½ teaspoon (1.5 grams) kosher salt\n    2 cups (400 grams) granulated sugar, divIded\n    ¾ cup (180 grams) fresh lemon juice\n    6 large pasteurized egg yolks (120 grams)\n    2 (8-ounce) containers (452 grams) cold BelGioioso Mascarpone Cheese\n\nGarnish: fresh blueberries",
						Instructions = "Bluebrerry Compote\nIn a medium saucepan, combine blueberries, sugar, lemon juice, 1 tablespoon (15 grams) water, vanilla, and salt. Heat over medium-high heat, stirring occasionally, until blueberries begin to break down and mixture begins to boil. Cook, stirring occasionally, until thickened, about 10 minutes. Remove from heat, and let cool completely. Refrigerate in an airtight container for up to 1 week. \n\nLadyfingers\nPreheat oven to 375°F (190°C). Line 2 baking sheets with parchment paper.\n\nIn a small bowl, sift together flour and cornstarch.\n\nIn the bowl of a stand mixer fitted with the whisk attachment, beat egg whites at medium speed until foamy, about 30 seconds. With mixer on medium speed, add 1⁄4 cup (50 grams) granulated sugar in a slow, steady stream. Increase mixer speed to medium-high, and beat until stiff peaks form, 4 to 5 minutes. Transfer to a medium bowl.\n\nClean bowl of stand mixer and whisk attachment. Using the whisk attachment, beat egg yolks and ¼ cup (50 grams) granulated sugar at medium-high speed until pale yellow and thick, 2 to 3 minutes. Using a balloon whisk, fold half of egg white mixture and vanilla into egg yolk mixture just until combined; fold in remaining egg white mixture. Fold in sifted flour mixture in two additions just until combined.\n\nWorking in batches if necessary, transfer batter to a large pastry bag fitted with a 7⁄16-inch round piping tip (Ateco #805). Pipe 3-inch-long lines at least 1 inch apart on prepared pans. (Final dimensions of Ladyfingers should be 3x1 inches.)\n \nIn a small bowl, whisk together confectioners’ sugar and remaining 2 tablespoons (24 grams) granulated sugar. Using a fine-mesh sieve, lightly dust piped batter with sugars.\n\nBake for 4 minutes. Rotate pans between racks, and bake until light golden brown and dry, 3 to 4 minutes more. Let cool completely. Best used within 24 hours.\n\nBlueberry-Lemon Tiramisù\nIn the work bowl of a food processor, pulse Blueberry Compote until smooth.\n\nIn the bowl of a stand mixer fitted with the whisk attachment, beat cold cream, lemon zest, vanilla, and salt at medium- high speed until stiff peaks form, about 2 minutes. Transfer to a large bowl; cover and refrigerate. Clean bowl of stand mixer and whisk attachment.\n\nIn a small saucepan, bring ¾ cup (150 grams) granulated sugar and lemon juice to a boil over medium-high heat, stirring until sugar dissolves, about 1 minute. Pour into a medium heatproof bowl, and let cool completely.\n\nIn the heatproof bowl of a stand mixer, whisk together egg yolks and remaining 1¼ cups (250 grams) sugar. Place bowl over a saucepan of simmering water. Cook, whisking frequently, until an instant-read thermometer registers 110°F (43°C).\n\nWipe bottom of stand mixer bowl dry; carefully return bowl to stand mixer. Using the whisk attachment, beat at medium-high speed until thick and ribbon- consistency, 2 to 3 minutes. (Mixture will still have texture from the sugar.) Let cool for 5 to 10 minutes.\n\nIn a medium bowl, stir cold mascarpone with a spatula until softened, smooth, and creamy; fold into egg yolk mixture in two additions just until combined. Fold whipped cream into mascarpone mixture in two additions just until combined.\n\nQuickly dip half of Ladyfingers in lemon syrup. (Do not let it soak.) Place in bottom of a 13x9-inch baking dish in a single layer, with long sIdes of Ladyfingers going along long sIdes of pan. (Trim or cut Ladyfingers as needed to fully cover bottom of pan.) Spread half of mascarpone mixture (about 4 cups or 630 grams) onto dipped Ladyfingers. Spread 3/4 cup (150 grams) Blueberry Compote onto mascarpone mixture.\n\nDip remaining Ladyfingers in lemon syrup, and layer on compote in pan. Spread remaining mascarpone mixture onto Ladyfingers to fully cover. Dollop remaining compote onto mascarpone mixture; using a wooden pick, swirl compote as desired. Cover and refrigerate for 24 hours before serving. Just before serving, garnish with blueberries, if desired. Cover and refrigerate for up to 3 days.",
						Image = "https://bakefromscratch.com/wp-content/uploads/2023/05/Blueberry-Tiramisu-1464-KC_1-e1683478018399-696x696.jpg",
					}
				}
			);
				recipeContext.SaveChanges();

				prePopulated = true;
			}
		}
	}
}