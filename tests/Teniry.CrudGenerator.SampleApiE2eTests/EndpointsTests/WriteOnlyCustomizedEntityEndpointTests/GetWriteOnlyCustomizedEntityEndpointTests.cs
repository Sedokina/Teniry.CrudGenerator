using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.EndpointsTests.WriteOnlyCustomizedEntityEndpointTests;

public class GetWriteOnlyCustomizedEntityEndpointTests {
    [Theory]
    [InlineData("GetWriteOnlyCustomizedEntityEndpoint")]
    public void Should_NotGenerateEndpointClass(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}