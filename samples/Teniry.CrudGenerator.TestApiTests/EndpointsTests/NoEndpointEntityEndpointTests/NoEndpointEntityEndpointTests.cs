using Teniry.CrudGenerator.TestApiTests.E2eTests.Core;

namespace Teniry.CrudGenerator.TestApiTests.EndpointsTests.NoEndpointEntityEndpointTests;

public class NoEndpointEntityEndpointTests {
    [Theory]
    [InlineData("GetNoEndpointEntityEndpoint")]
    [InlineData("GetNoEndpointEntitiesEndpoint")]
    [InlineData("CreateNoEndpointEntityEndpoint")]
    [InlineData("UpdateNoEndpointEntityEndpoint")]
    [InlineData("DeleteNoEndpointEntityEndpoint")]
    public void Should_NotGenerateEndpointClass(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}