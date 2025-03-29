using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NSwagWithExamples.Models;

namespace NSwagWithExamplesMinimal.Routes;

public static class CityRouteExtension
{
    public static RouteGroupBuilder MapCityApi(this RouteGroupBuilder endpoints)
    {
        var cities = endpoints.MapGroup("cities").WithTags("Cities");
        cities.MapGet("", GetCities).WithName("Get cities").WithOpenApi();
        cities.MapGet("{id:int}", GetCity).WithName("Get city").WithOpenApi();
        cities.MapDelete("{id:int}", DeleteCity).WithName("Delete city").WithOpenApi();
        return endpoints;
    }
    
    private static Ok<List<City>> GetCities() => TypedResults.Ok(new List<City>());

    private static Ok<City> GetCity([FromRoute] int id) => TypedResults.Ok(new City());

    private static Ok DeleteCity([FromRoute] int id) => TypedResults.Ok();
}