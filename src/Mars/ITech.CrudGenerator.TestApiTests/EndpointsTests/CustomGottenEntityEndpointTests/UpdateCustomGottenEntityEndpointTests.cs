using ITech.CrudGenerator.TestApiTests.E2eTests.Core;

namespace ITech.CrudGenerator.TestApiTests.EndpointsTests.CustomGottenEntityEndpointTests;

public class UpdateCustomGottenEntityEndpointTests {
    [Theory]
    [InlineData("UpdateCustomGottenEntityEndpoint")]
    public void Should_NotGenerateEndpointClass(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}