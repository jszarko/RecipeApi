using Microsoft.EntityFrameworkCore;
using RecipeApi.Models;
using RecipeApi.Services;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

var CorsWhitelistPolicy = "corsWhitelistPolicy";

// Add services to the container.
builder.Services.AddCors(options =>
{
	options.AddPolicy(name: CorsWhitelistPolicy,
		policy =>
		{
			policy.WithOrigins(
				"http://localhost:3000",
				"https://localhost:3000")
					.AllowAnyHeader()
					.AllowAnyMethod();
		});
});

builder.Services.AddControllers();
builder.Services.AddTransient<RecipeService>();
//builder.Services.AddDbContext<RecipeContext>(opt =>
//	opt.UseInMemoryDatabase("RecipeList"));
builder.Services.AddDbContext<RecipeContext>(opt =>
	opt.UseSqlServer(@"Server=tcp:localhost,1433;Initial Catalog=recipe_manager;Persist Security Info=False;User ID=sa_recipe;Password=9s$7dFIm#nvaP!Eq$bE8;TrustServerCertificate=True;"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(CorsWhitelistPolicy);
app.MapControllers();
app.UseAuthorization();

app.Run();
