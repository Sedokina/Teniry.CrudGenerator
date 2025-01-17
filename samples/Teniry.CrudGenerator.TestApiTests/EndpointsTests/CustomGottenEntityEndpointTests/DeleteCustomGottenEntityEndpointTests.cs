using Teniry.CrudGenerator.TestApiTests.E2eTests.Core;

namespace Teniry.CrudGenerator.TestApiTests.EndpointsTests.CustomGottenEntityEndpointTests;

public class DeleteCustomGottenEntityEndpointTests {
    [Theory]
    [InlineData("DeleteCustomGottenEntityEndpoint")]
    public void Should_NotGenerateEndpointClass(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}