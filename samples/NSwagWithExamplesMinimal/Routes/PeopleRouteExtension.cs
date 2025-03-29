using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NSwag.Examples;
using NSwagWithExamples.Models;
using NSwagWithExamples.Models.Examples.Persons.Requests;

namespace NSwagWithExamplesMinimal.Routes;

public static class PeopleRouteExtension    
{
    private static readonly ConcurrentDictionary<int, Person> People = new();
    private static readonly object Lock = new();

    private static void NewPerson(Person person)
    {
        lock (Lock)
        {
            person.Id = People.Keys.Count != 0 ? People.Keys.Max() + 1 : 1;
            People.TryAdd(person.Id, person);
        }
    }

    static PeopleRouteExtension()
    {
        NewPerson(new Person("Franta", "Jetel"));
        NewPerson(new Person("Jára", "Cimrman"));
        NewPerson(new Person("Jindra", "Hlaváček"));
        NewPerson(new Person("Vilma", "Böhmová"));
        NewPerson(new Person("Emanuel", "Pecháček"));
        NewPerson(new Person("Inspektor", "Trachta"));
        NewPerson(new Person("Inspektor", "Klečka"));
        NewPerson(new Person("první", "podezřelý"));
        NewPerson(new Person("druhý", "podezřelý"));
        NewPerson(new Person("třetí", "podezřelý"));
        NewPerson(new Person("čtvrtý", "podezřelý"));
    }
    
    public static RouteGroupBuilder MapPeopleApi(this RouteGroupBuilder endpoints)
    {
        var people = endpoints.MapGroup("people").WithTags("People");
        people.MapGet("", GetPeople).WithName("Get people");
        people.MapPost("", CreatePerson).WithName("Create person");
        people.MapGet("count", GetNumberOfPeople).WithName("Get number of people");
        people.MapGet("{id:int}", GetPerson).WithName("Get person");
        people.MapGet("{id:int}/age", GetPersonAge).WithName("Get persons age");
        people.MapGet("{id:int}/birth", GetPersonBirth).WithName("Get persons birth");
        people.MapPost("from-file", CreatePersonFromFile).WithName("Create person from file");
        return endpoints;
    }

    [EndpointSpecificExample(typeof(PersonAge18Example), typeof(PersonAge69Example), ParameterName = "minAge",
        ExampleType = ExampleType.Request)]
    [EndpointSpecificExample(typeof(PersonTextExample1), typeof(PersonTextExample2), typeof(PersonTextExample3),
        ParameterName = "searchText", ExampleType = ExampleType.Request)]
    private static Ok<List<Person>> GetPeople([FromQuery] int? minAge = null, [FromQuery] string searchText = null)
    {
        if (minAge == null && string.IsNullOrEmpty(searchText))
            return TypedResults.Ok(People.Values.ToList());

        if (minAge == null)
            return TypedResults.Ok(People.Values.Where(p =>
                    $"{p.FirstName}~{p.LastName}".Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                .ToList());

        if (string.IsNullOrEmpty(searchText))
            return TypedResults.Ok(People.Values.Where(p => p.Age >= minAge).ToList());

        return TypedResults.Ok(People.Values.Where(p =>
            p.Age >= minAge &&
            $"{p.FirstName}~{p.LastName}".Contains(searchText, StringComparison.CurrentCultureIgnoreCase)).ToList());
    }

    private static Ok<int> GetNumberOfPeople() => TypedResults.Ok(People.Count);

    private static Results<Ok<Person>, NotFound> GetPerson([FromRoute] int id) =>
        People.TryGetValue(id, out var person) ? TypedResults.Ok(person) : TypedResults.NotFound();

    private static Results<Ok<int>, NotFound> GetPersonAge([FromRoute] int id) =>
        People.TryGetValue(id, out var person) ? TypedResults.Ok(person.Age) : TypedResults.NotFound();

    private static Results<Ok<DateTime>, NotFound>  GetPersonBirth([FromRoute] int id) =>
        People.TryGetValue(id, out var person) ? TypedResults.Ok(person.BirthDay) : TypedResults.NotFound();

    private static Results<Ok<int>, BadRequest<string>> CreatePerson([FromBody] Person person)
    {
        if (person == null)
            return TypedResults.BadRequest("{person} is null");

        if (person.BirthDay.Year < 1800 || person.BirthDay > DateTime.Now)
            return TypedResults.BadRequest($"BirthDay is out of range (1800/01/01..today)");

        NewPerson(person);
        return TypedResults.Ok(person.Id);
    }

    private static Ok CreatePersonFromFile([FromForm] IFormFile file) => TypedResults.Ok();
}
