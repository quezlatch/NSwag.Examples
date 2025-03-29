using System.Net.Mime;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NSwagWithExamples.Models.Zoo;
using RandomNameGeneratorLibrary;

namespace NSwagWithExamplesMinimal.Routes;

public static class ZooRouteExtension
{
    public static RouteGroupBuilder MapZooApi(this RouteGroupBuilder endpoints)
    {
        var zoos = endpoints.MapGroup("zoos").WithTags("Zoos");
        zoos.MapGet("", GetAnimals).WithName("Get animals");
        zoos.MapPost("", Adopt).WithName("Adopt animal")
            .Accepts<Animal>(MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status201Created);
        zoos.MapGet("{name}", GetAnimal).WithName("Get animal");
        return endpoints;
    }

    private static Ok<Animal[]> GetAnimals() => TypedResults.Ok(Array.Empty<Animal>());
    
    private static Ok<Animal> GetAnimal([FromRoute] string name) => TypedResults.Ok(default(Animal));
    
    private static async Task<IResult> Adopt(HttpContext context, IPersonNameGenerator _)
    {
        var animal = await context.Request.ReadFromJsonAsync<Animal>();
        if (animal == null) throw new InvalidOperationException("Could not deserialise animal");
        return TypedResults.Created();
    }
}