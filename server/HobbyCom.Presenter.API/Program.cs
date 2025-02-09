using System.Reflection;
using HobbyCom.Presenter.API;
using HobbyCom.Presenter.API.src.Middlewares;
using HobbyCom.Presenter.API.src.Utilities;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(
        new SlugifyParameterTransformer())); // Converts route names to lowercase, kebab-case
});

// Register all project dependencies
builder.Services.AddProjectDependencies(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HobbyCom API",
        Version = "v1"
    });

    // Include XML comments from all relevant projects
    var xmlFiles = new[]
    {
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml",
        "HobbyCom.Application.xml",
        "HobbyCom.Infrastructure.xml",
        "HobbyCom.Presenter.API.xml"
    };

    foreach (var xmlFile in xmlFiles)
    {
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            setup.IncludeXmlComments(xmlPath);
        }
    }
});

builder.Services.AddRouting();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAPI");
        c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
    });
    app.MapGet("/swagger-ui/SwaggerDark.css", async (CancellationToken cancellationToken) =>
    {
        var css = await File.ReadAllBytesAsync("SwaggerDark.css", cancellationToken).ConfigureAwait(false);
        return Results.File(css, "text/css");
    }).ExcludeFromDescription();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();
app.MapControllers();
app.Run();

