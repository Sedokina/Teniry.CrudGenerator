using Teniry.CrudGenerator.TestApiTests.E2eTests.Core;

namespace Teniry.CrudGenerator.TestApiTests.HandlersTests.CustomGottenEntityHandlerTests;

public class CreateCustomGottenEntityHandlerTests {
    [Theory]
    [InlineData("CreateCustomGottenEntityCommand")]
    [InlineData("CreateCustomGottenEntityHandler")]
    public void Should_NotGenerateCreateHandler(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}