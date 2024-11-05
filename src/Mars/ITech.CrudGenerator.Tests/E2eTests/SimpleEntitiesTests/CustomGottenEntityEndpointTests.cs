using System.Net;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.Tests.E2eTests.Core;

namespace ITech.CrudGenerator.Tests.E2eTests.SimpleEntitiesTests;

[Collection("E2eTests")]
public class CustomGottenEntityEndpointTests(TestApiFixture fixture)
{
    private readonly TestMongoDb _db = fixture.GetDb();
    private readonly HttpClient _httpClient = fixture.GetHttpClient();
    
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