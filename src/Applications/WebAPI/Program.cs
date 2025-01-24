using MediatR;
using Microsoft.OpenApi.Models;
using WebAPI.Infrastructure.Helpers;
using WebAPI.Infrastructure.Interfaces.Helpers;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    EnvironmentName = environment
});
var configuration = builder.Configuration;

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);
});

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Desafio LuizaLabs",
        Version = "v1",
        Description = "API criada para consulta de Pedidos",
        Contact = new OpenApiContact
        {
            Name = "Luiz Faitanin",
            Email = "lfaitanin@outlook.com",
        }
    });

    c.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
});

builder.Services.AddControllers();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddScoped<IFileReader, FileReader>();
builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Desafio LuizaLabs v1");
        c.RoutePrefix = "swagger"; 
    });
}

app.MapControllers();
app.Run();