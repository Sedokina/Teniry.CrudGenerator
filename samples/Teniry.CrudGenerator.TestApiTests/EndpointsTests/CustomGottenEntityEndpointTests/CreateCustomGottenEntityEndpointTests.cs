using Teniry.CrudGenerator.TestApiTests.E2eTests.Core;

namespace Teniry.CrudGenerator.TestApiTests.EndpointsTests.CustomGottenEntityEndpointTests;

public class CreateCustomGottenEntityEndpointTests {
    [Theory]
    [InlineData("CreateCustomGottenEntityEndpoint")]
    public void Should_NotGenerateEndpointClass(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}