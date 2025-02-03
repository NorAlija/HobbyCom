using HobbyCom.Presenter.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Register all project dependencies
builder.Services.AddProjectDependencies(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello world!"); //TODO: To be delete later

app.UseHttpsRedirection();
app.Run();

