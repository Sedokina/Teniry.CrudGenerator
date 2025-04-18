using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.ReadOnlyCustomizedEntityHandlerTests;

public class PatchReadOnlyCustomizedEntityHandlerTests {
    [Theory]
    [InlineData("PatchReadOnlyCustomizedEntityCommand")]
    [InlineData("PatchReadOnlyCustomizedEntityHandler")]
    public void Should_NotGenerateUpdateHandler(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}