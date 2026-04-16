using Pointmaster.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IPostRepository, DummyPostRepository>();
builder.Services.AddSingleton<IPatruljeRepository, DummyPatruljeRepository>();
builder.Services.AddSingleton<IPointRepository, DummyPointRepository>();

var app = builder.Build();

app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapFallbackToFile("index.html");

app.Run();
