using ITech.CrudGenerator.Tests.E2eTests.Core;

namespace ITech.CrudGenerator.Tests.HandlersTests.CustomGottenEntityHandlerTests;

public class CreateCustomGottenEntityHandlerTests
{
    [Theory]
    [InlineData("CreateCustomGottenEntityCommand")]
    [InlineData("CreateCustomGottenEntityHandler")]
    public void Should_NotGenerateCreateHandler(string typeName)
    {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}