using ITech.CrudGenerator.Tests.E2eTests.Core;

namespace ITech.CrudGenerator.Tests.HandlersTests.CustomGottenEntityHandlerTests;

public class DeleteCustomGottenEntityHandlerTests
{
    [Theory]
    [InlineData("DeleteCustomGottenEntityCommand")]
    [InlineData("DeleteCustomGottenEntityHandler")]
    public void Should_NotGenerateDeleteHandler(string typeName)
    {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}