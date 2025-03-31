using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.ReadOnlyCustomizedEntityHandlerTests;

public class UpdateReadOnlyCustomizedEntityHandlerTests {
    [Theory]
    [InlineData("UpdateCustomGottenEntityCommand")]
    [InlineData("UpdateCustomGottenEntityHandler")]
    public void Should_NotGenerateUpdateHandler(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}