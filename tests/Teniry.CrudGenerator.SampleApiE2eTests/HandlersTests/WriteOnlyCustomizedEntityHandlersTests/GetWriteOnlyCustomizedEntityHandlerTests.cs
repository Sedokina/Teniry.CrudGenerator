using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.WriteOnlyCustomizedEntityHandlersTests;

public class GetWriteOnlyCustomizedEntityHandlerTests {
    [Theory]
    [InlineData("GetWriteOnlyCustomizedEntityQuery")]
    [InlineData("GetWriteOnlyCustomizedEntityHandler")]
    public void Should_NotGenerateGetHandler(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().NotContainType(typeName);
    }
}