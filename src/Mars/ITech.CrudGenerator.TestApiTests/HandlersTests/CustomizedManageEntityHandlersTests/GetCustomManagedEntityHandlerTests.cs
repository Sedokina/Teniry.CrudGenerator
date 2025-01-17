using ITech.CrudGenerator.TestApiTests.E2eTests.Core;

namespace ITech.CrudGenerator.TestApiTests.HandlersTests.CustomizedManageEntityHandlersTests;

public class GetCustomManagedEntityHandlerTests {
    [Theory]
    [InlineData("GetCustomManagedEntityQuery")]
    [InlineData("GetCustomManagedEntityHandler")]
    public void Should_NotGenerateGetHandler(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}