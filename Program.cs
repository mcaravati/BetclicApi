using BetclicApi.Models;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<BetclicContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SwaggerDemo", Version = "v1" });

    var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "SwaggerDemo.xml");

    c.IncludeXmlComments(filePath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();