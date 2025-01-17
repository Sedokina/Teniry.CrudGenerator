using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.EndpointsTests.CustomManagedEntityEndpointTests;

public class GetCustomManagedEntitiesListEndpointTests {
    [Theory]
    [InlineData("GetCustomManagedEntitiesEndpoint")]
    public void Should_NotGenerateEndpointClass(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}