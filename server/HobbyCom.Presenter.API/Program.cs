using System.Reflection;
using HobbyCom.Presenter.API;
using HobbyCom.Presenter.API.src.Middlewares;
using HobbyCom.Presenter.API.src.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new SpinCaseTransformer()));
});

// Define the directory path for keys
string keyDirectory = Path.Combine(builder.Environment.ContentRootPath, "./");

// Instantiate GenerateKeyPairs
// This will generate and save the key pair upon application startup
var keyGenerator = new GenerateKeyPairs(keyDirectory);


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

    setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. 
                      Add your token in the text input below.
                      Example: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        // Scheme = JwtBearerDefaults.AuthenticationScheme,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    // Apply the security scheme globally
    setup.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddRouting();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policyBuilder =>
    {
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

        if (allowedOrigins != null && allowedOrigins.Length > 0)
        {
            policyBuilder.WithOrigins(allowedOrigins)
                         .AllowAnyMethod()
                         .AllowAnyHeader()
                         .AllowCredentials();
        }
    });
});

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

// app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

