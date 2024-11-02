using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;
using ITech.CrudGenerator.TestApi.Application.SimpleEntityFeature.GetSimpleEntities;
using ITech.CrudGenerator.Tests.e2eTests.Core;

namespace ITech.CrudGenerator.Tests.e2eTests.SimpleEntitiesTests;

[Collection("E2eTests")]
public class SimpleEntityListEndpointTests(TestApiFixture fixture)
{
    private readonly HttpClient _httpClient = fixture.GetHttpClient();

    public static TheoryData<string, string, Expression<Func<SimpleEntitiesListItemDto, object>>> SortData => new()
    {
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

    public static TheoryData<string, Expression<Func<SimpleEntitiesListItemDto, bool>>> FilterData => new()
    {
        { "simpleEntity?page=1&pageSize=10&name=First", x => x.Name.Contains("First") },
    };
    
    [Theory]
    [MemberData(nameof(FilterData))]
    public async Task Should_FilterResult(string endpoint, Expression<Func<SimpleEntitiesListItemDto, bool>> validation)
    {
        // Act
        var response = await _httpClient.GetAsync(endpoint);
        response.Should().FailIfNotSuccessful();
    
        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    
        var actual = await response.Content.ReadFromJsonAsync<SimpleEntitiesDto>();
        actual!.Items.Should().HaveCountGreaterThan(0).And.OnlyContain(validation);
    }
}