using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.EndpointsTests.WriteOnlyCustomizedEntityEndpointTests;

public class GetWriteOnlyCustomizedEntityListEndpointTests {
    [Theory]
    [InlineData("GetWriteOnlyCustomizedEntitiesEndpoint")]
    public void Should_NotGenerateEndpointClass(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}