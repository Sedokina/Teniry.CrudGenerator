using ITech.CrudGenerator.Tests.E2eTests.Core;

namespace ITech.CrudGenerator.Tests.HandlersTests.CustomizedManageEntityHandlersTests;

public class GetCustomManagedEntityHandlerTests
{
    [Theory]
    [InlineData("GetCustomManagedEntityQuery")]
    [InlineData("GetCustomManagedEntityHandler")]
    public void Should_NotGenerateGetHandler(string typeName)
    {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}