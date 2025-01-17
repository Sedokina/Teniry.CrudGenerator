using System.Net;
using ITech.CrudGenerator.TestApiTests.E2eTests.Core;

namespace ITech.CrudGenerator.TestApiTests.E2eTests.CustomEntitiesTests;

[Collection("E2eTests")]
public class NoEndpointEntityEndpointTests(TestApiFixture fixture) {
    private readonly HttpClient _httpClient = fixture.GetHttpClient();

    [Theory]
    [InlineData("noEndpointEntity/acda862c-c49f-4ea6-84c2-e5783dce8bc1")]
    [InlineData("noEndpointEntity")]
    [InlineData("noEndpointEntity/create")]
    [InlineData("noEndpointEntity/acda862c-c49f-4ea6-84c2-e5783dce8bc1/delete")]
    [InlineData("noEndpointEntity/acda862c-c49f-4ea6-84c2-e5783dce8bc1/update")]
    public async Task Should_NotGenerateManageEndpoints(string endpoint) {
        // Act
        var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Options, endpoint));

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}