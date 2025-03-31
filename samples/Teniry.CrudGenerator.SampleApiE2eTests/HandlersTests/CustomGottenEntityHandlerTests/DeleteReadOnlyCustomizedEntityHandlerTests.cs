using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.CustomGottenEntityHandlerTests;

public class DeleteReadOnlyCustomizedEntityHandlerTests {
    [Theory]
    [InlineData("DeleteCustomGottenEntityCommand")]
    [InlineData("DeleteCustomGottenEntityHandler")]
    public void Should_NotGenerateDeleteHandler(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}