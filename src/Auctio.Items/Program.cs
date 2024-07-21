using Auctio.Items;
using Auctio.Shared.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddJwt();
builder.Services.ConfigureDbSource(builder.Configuration);
builder.Services.ConfigureCache(builder.Configuration);
builder.Services.AddServices();

var app = builder.Build();


// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

