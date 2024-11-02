using System.Net;
using System.Net.Http.Json;
using ITech.CrudGenerator.TestApi.Application.SimpleTypeDefaultSortEntityFeature.GetSimpleTypeDefaultSortEntities;
using ITech.CrudGenerator.Tests.Endpoints.Core;

namespace ITech.CrudGenerator.Tests.Endpoints.SimpleEntitiesTests;

[Collection("E2eTests")]
public class SimpleTypeListEndpointTests(TestApiFixture fixture)
{
    private readonly HttpClient _httpClient = fixture.GetHttpClient();

    [Theory]
    [InlineData("simpleTypeDefaultSortEntity?page=1&pageSize=10")]
    public async Task Should_SortListResult(string endpoint)
    {
        // Act
        var response = await _httpClient.GetAsync(endpoint);
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<SimpleTypeDefaultSortEntitiesDto>();

        actual!.Items.Should().HaveCountGreaterThan(0)
            .And.BeInDescendingOrder(x => x.Name);
    }
}