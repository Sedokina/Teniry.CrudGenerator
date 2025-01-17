using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.CustomizedManageEntityHandlersTests;

public class GetCustomManagedEntitiesListHandlerTests {
    [Theory]
    [InlineData("GetCustomManagedEntitiesQuery")]
    [InlineData("GetCustomManagedEntitiesHandler")]
    public void Should_NotGenerateGetHandler(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}