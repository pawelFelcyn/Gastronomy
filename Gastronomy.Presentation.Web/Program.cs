using Gastronomy.Backend.Database;
using Gastronomy.Presentation.Web;
using Gastronomy.Presentation.Web.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var dbConnectionString = builder.Configuration.GetConnectionString("Database");
builder.Services.AddDbContext<GastronomyDbContext>(options => options.UseSqlServer(
    dbConnectionString, x => x.MigrationsAssembly("Gastronomy.Backend.Database.MSSQL")));

builder.Services
    .AddMudServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GastronomyDbContext>();
    dbContext.Database.EnsureCreated();
    await dbContext.Database.MigrateAsync();

    await DataSeeder.SeedData(dbContext);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
