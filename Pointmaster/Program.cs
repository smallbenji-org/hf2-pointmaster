using DbUp;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Pointmaster;
using Pointmaster.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
var configs = builder.Configuration.GetSection("PointMaster");
builder.Services.Configure<PointMasterConfig>(configs);

builder.Services.AddControllers();

builder.Services.AddSingleton<IPostRepository, PostRepository>();
builder.Services.AddSingleton<IPatruljeRepository, PatruljeRepository>();
builder.Services.AddSingleton<IPointRepository, PointRepository>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireDigit = false;
    options.SignIn.RequireConfirmedAccount = false;
});

var app = builder.Build();

app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapFallbackToFile("index.html");

DeployChanges
    .To
    .PostgresqlDatabase(app.Services.GetRequiredService<IOptions<PointMasterConfig>>().Value.ConnectionString)
    .WithScriptsFromFileSystem("Migrations")
    .LogToConsole()
    .Build()
    .PerformUpgrade();


Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

app.Run();
