using ITech.CrudGenerator.Tests.E2eTests.Core;

namespace ITech.CrudGenerator.Tests.EndpointsTests.CustomManagedEntityEndpointTests;

public class GetCustomManagedEntitiesListEndpointTests
{
    [Theory]
    [InlineData("GetCustomManagedEntitiesEndpoint")]
    public void Should_NotGenerateEndpointClass(string typeName)
    {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}