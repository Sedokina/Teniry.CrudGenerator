using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.CustomGottenEntityHandlerTests;

public class UpdateCustomGottenEntityHandlerTests {
    [Theory]
    [InlineData("UpdateCustomGottenEntityCommand")]
    [InlineData("UpdateCustomGottenEntityHandler")]
    public void Should_NotGenerateUpdateHandler(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}