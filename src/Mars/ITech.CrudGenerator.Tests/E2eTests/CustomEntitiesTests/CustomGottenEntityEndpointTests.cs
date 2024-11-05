using System.Net;
using System.Net.Http.Json;
using ITech.CrudGenerator.TestApi.Application.CustomGottenEntityFeature.CustomGottenEntityGetListOperationCustomNs;
using ITech.CrudGenerator.TestApi.Application.CustomGottenEntityFeature.CustomGottenEntityGetOperationCustomNs;
using ITech.CrudGenerator.Tests.E2eTests.Core;

namespace ITech.CrudGenerator.Tests.E2eTests.CustomEntitiesTests;

[Collection("E2eTests")]
public class CustomGottenEntityEndpointTests(TestApiFixture fixture)
{
    private readonly HttpClient _httpClient = fixture.GetHttpClient();
    
    [Theory]
    [InlineData("getCustomGottenEntityById/{0}")]
    public async Task Should_GetEntity(string endpoint)
    {
        // Arrange
        var entityId = new Guid("27ed3a08-c92e-4c8d-b515-f793eb65cacd");
        
        // Act
        var response = await _httpClient.GetAsync(string.Format(endpoint, entityId));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<CustomizedNameGetCustomEntityDto>();
        actual.Should().NotBeNull();
        actual!.Id.Should().Be(entityId);
        actual.Name.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("getAllCustomGottenEntitiesList?page=1&pageSize=10")]
    public async Task Should_GetEntitiesList(string endpoint)
    {
        // Act
        var response = await _httpClient.GetAsync(endpoint);
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<CustomizedNameGetCustomEntitiesListDto>();
        actual.Should().NotBeNull();
        actual!.Page.PageSize.Should().BeGreaterThan(0);
        actual.Page.CurrentPageIndex.Should().BeGreaterThan(0);
        actual.Items.Should().HaveCountGreaterThanOrEqualTo(1);
        actual.Items.Should().AllSatisfy(x =>
        {
            x.Id.Should().NotBeEmpty();
            x.Name.Should().NotBeNullOrEmpty();
        });
    }
    
    [Theory]
    [InlineData("POST", "customGottenEntity/create")]
    [InlineData("DELETE", "customGottenEntity/acda862c-c49f-4ea6-84c2-e5783dce8bc1/delete")]
    [InlineData("PUT", "customGottenEntity/acda862c-c49f-4ea6-84c2-e5783dce8bc1/update")]
    public async Task Should_NotGenerateManageEndpoints(string httpMethod, string endpoint)
    {
        // Act
        var response = await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod(httpMethod), endpoint));

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}