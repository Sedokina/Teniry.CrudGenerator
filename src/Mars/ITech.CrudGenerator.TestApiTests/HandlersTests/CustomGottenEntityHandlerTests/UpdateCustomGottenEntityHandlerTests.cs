using ITech.CrudGenerator.TestApiTests.E2eTests.Core;

namespace ITech.CrudGenerator.TestApiTests.HandlersTests.CustomGottenEntityHandlerTests;

public class UpdateCustomGottenEntityHandlerTests {
    [Theory]
    [InlineData("UpdateCustomGottenEntityCommand")]
    [InlineData("UpdateCustomGottenEntityHandler")]
    public void Should_NotGenerateUpdateHandler(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}