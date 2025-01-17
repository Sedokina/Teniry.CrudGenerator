using Teniry.CrudGenerator.TestApiTests.E2eTests.Core;

namespace Teniry.CrudGenerator.TestApiTests.HandlersTests.CustomGottenEntityHandlerTests;

public class DeleteCustomGottenEntityHandlerTests {
    [Theory]
    [InlineData("DeleteCustomGottenEntityCommand")]
    [InlineData("DeleteCustomGottenEntityHandler")]
    public void Should_NotGenerateDeleteHandler(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}