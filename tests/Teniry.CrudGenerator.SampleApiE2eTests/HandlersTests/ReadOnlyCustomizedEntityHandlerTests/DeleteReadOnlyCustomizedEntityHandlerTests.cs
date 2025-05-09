using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.ReadOnlyCustomizedEntityHandlerTests;

public class DeleteReadOnlyCustomizedEntityHandlerTests {
    [Theory]
    [InlineData("DeleteReadOnlyCustomizedEntityCommand")]
    [InlineData("DeleteReadOnlyCustomizedEntityHandler")]
    public void Should_NotGenerateDeleteHandler(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}