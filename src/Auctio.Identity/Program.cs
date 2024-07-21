using Auctio.Identity;
using Auctio.Shared.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddJwt();
builder.Services.ConfigureDbSource(builder.Configuration);
builder.Services.ConfigureMassTransit();
builder.Services.AddServices();

var app = builder.Build();


// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
