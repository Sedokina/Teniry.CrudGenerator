using System.Net;
using System.Net.Http.Json;
using Teniry.CrudGenerator.SampleApi.Application.ReadOnlyCustomizedEntityFeature.GetReadOnlyModelsListCustomNamespace;
using Teniry.CrudGenerator.SampleApi.Application.ReadOnlyCustomizedEntityFeature.GetReadOnlyModelCustomNamespace;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.CustomEntitiesTests;

[Collection("E2eTests")]
public class ReadOnlyCustomizedEntityEndpointTests(TestApiFixture fixture) {
    private readonly HttpClient _httpClient = fixture.GetHttpClient();

    [Theory]
    [InlineData("getCustomizedReadOnlyModelById/{0}")]
    public async Task Should_GetEntity(string endpoint) {
        // Arrange
        var entityId = new Guid("27ed3a08-c92e-4c8d-b515-f793eb65cacd");

        // Act
        var response = await _httpClient.GetAsync(string.Format(endpoint, entityId));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<ReadOnlyModelCustomDto>();
        actual.Should().NotBeNull();
        actual!.Id.Should().Be(entityId);
        actual.Name.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("getAllCustomizedReadOnlyEntities?page=1&pageSize=10")]
    public async Task Should_GetEntitiesList(string endpoint) {
        // Act
        var response = await _httpClient.GetAsync(endpoint);
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<ReadOnlyModelsListCustomDto>();
        actual.Should().NotBeNull();
        actual!.Page.PageSize.Should().BeGreaterThan(0);
        actual.Page.CurrentPageIndex.Should().BeGreaterThan(0);
        actual.Items.Should().HaveCountGreaterThanOrEqualTo(1);
        actual.Items.Should().AllSatisfy(
            x => {
                x.Id.Should().NotBeEmpty();
                x.Name.Should().NotBeNullOrEmpty();
            }
        );
    }

    [Theory]
    [InlineData("getAllCustomizedReadOnlyEntities?page=1&pageSize=10")]
    public async Task Should_SortListWithDefaultSort(string endpoint) {
        // Act
        var response = await _httpClient.GetAsync(endpoint);
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<ReadOnlyModelsListCustomDto>();

        actual!.Items.Should().HaveCountGreaterThan(0)
            .And.BeInDescendingOrder(x => x.Name);
    }

    [Theory]
    [InlineData("readOnlyCustomizedEntity/create")]
    [InlineData("readOnlyCustomizedEntity/acda862c-c49f-4ea6-84c2-e5783dce8bc1/delete")]
    [InlineData("readOnlyCustomizedEntity/acda862c-c49f-4ea6-84c2-e5783dce8bc1/update")]
    public async Task Should_NotGenerateManageEndpoints(string endpoint) {
        // Act
        var response = await _httpClient.SendAsync(new(HttpMethod.Options, endpoint));

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}