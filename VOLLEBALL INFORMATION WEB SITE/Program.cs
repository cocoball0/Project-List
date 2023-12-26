using Microsoft.EntityFrameworkCore;
using Mini_Project2.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var provider = builder.Services.BuildServiceProvider();
var config = provider.GetRequiredService<IConfiguration>();

builder.Services.AddDbContext<ClubDBContext>(item => item.UseSqlServer(config.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<PlayerDBcontext>(item => item.UseSqlServer(config.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<BoardDbContext>(item => item.UseSqlServer(config.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<NewsDbContext>(item => item.UseSqlServer(config.GetConnectionString("AnotherConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
