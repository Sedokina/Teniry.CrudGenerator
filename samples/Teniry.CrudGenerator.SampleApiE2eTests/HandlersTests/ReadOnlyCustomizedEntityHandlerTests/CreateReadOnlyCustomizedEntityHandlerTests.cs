using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.ReadOnlyCustomizedEntityHandlerTests;

public class CreateReadOnlyCustomizedEntityHandlerTests {
    [Theory]
    [InlineData("CreateCustomGottenEntityCommand")]
    [InlineData("CreateCustomGottenEntityHandler")]
    public void Should_NotGenerateCreateHandler(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}