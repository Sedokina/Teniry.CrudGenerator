using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.EndpointsTests.ReadOnlyCustomizedEntityEndpointTests;

public class DeleteReadOnlyCustomizedEntityEndpointTests {
    [Theory]
    [InlineData("DeleteCustomGottenEntityEndpoint")]
    public void Should_NotGenerateEndpointClass(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}