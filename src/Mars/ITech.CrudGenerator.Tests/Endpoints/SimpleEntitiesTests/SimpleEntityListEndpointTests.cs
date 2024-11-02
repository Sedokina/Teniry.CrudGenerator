using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.SimpleEntityFeature.GetSimpleEntities;
using ITech.CrudGenerator.TestApi.Generators.SimpleEntityGenerator;
using ITech.CrudGenerator.Tests.Endpoints.Core;

namespace ITech.CrudGenerator.Tests.Endpoints.SimpleEntitiesTests;

[Collection("E2eTests")]
public class SimpleEntityListEndpointTests(TestApiFixture fixture) : IAsyncLifetime
{
    private readonly TestMongoDb _db = fixture.GetDb();
    private readonly HttpClient _httpClient = fixture.GetHttpClient();

    public async Task InitializeAsync()
    {
        await _db.AddRangeAsync([
            new SimpleEntity { Id = Guid.NewGuid(), Name = "First Entity Name" },
            new SimpleEntity { Id = Guid.NewGuid(), Name = "Second Entity Name" }
        ]);
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
    }

    public static TheoryData<string, string, Expression<Func<SimpleEntitiesListItemDto, object>>> SortData => new()
    {
        { "simpleEntity?page=1&pageSize=10&sort=asc.id", "asc", x => x.Id },
        { "simpleEntity?page=1&pageSize=10&sort=desc.id", "desc", x => x.Id },
        { "simpleEntity?page=1&pageSize=10&sort=asc.name", "asc", x => x.Name },
        { "simpleEntity?page=1&pageSize=10&sort=desc.name", "desc", x => x.Name },
    };

    [Theory]
    [MemberData(nameof(SortData))]
    public async Task Should_SortListResult(
        string endpoint,
        string direction,
        Expression<Func<SimpleEntitiesListItemDto, object>> property)
    {
        // Act
        var response = await _httpClient.GetAsync(endpoint);
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<SimpleEntitiesDto>();
        if (direction.Equals("asc"))
        {
            actual!.Items.Should().BeInAscendingOrder(property);
        }
        else
        {
            actual!.Items.Should().BeInDescendingOrder(property);
        }
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}