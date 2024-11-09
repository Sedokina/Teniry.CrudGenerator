using ITech.CrudGenerator.Tests.E2eTests.Core;

namespace ITech.CrudGenerator.Tests.HandlersTests.CustomGottenEntityHandlerTests;

public class UpdateCustomGottenEntityHandlerTests
{
    [Theory]
    [InlineData("UpdateCustomGottenEntityCommand")]
    [InlineData("UpdateCustomGottenEntityHandler")]
    public void Should_NotGenerateUpdateHandler(string typeName)
    {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}