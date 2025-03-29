using NSwag.Examples;
using NSwagWithExamplesMinimal.Routes;
using NSwagWithExamples.Models.Examples;
using RandomNameGeneratorLibrary;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPersonNameGenerator, PersonNameGenerator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddExampleProviders(typeof(CitiesExample).Assembly);
builder.Services.AddOpenApiDocument((settings, provider) =>
{
    settings.Title = "NSwag with minimal api examples";
    settings.AddExamples(provider);
});

var app = builder.Build();

app.MapGroup("/api")
    .MapCityApi()
    .MapZooApi();

app.UseHttpsRedirection();

app.UseOpenApi();
app.UseSwaggerUi();

app.Run();