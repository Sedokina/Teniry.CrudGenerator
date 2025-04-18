using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Teniry.Cqrs.Commands;
using Teniry.CrudGenerator.SampleApi.Endpoints.WriteOnlyCustomizedEntityEndpoints;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.EndpointsTests.WriteOnlyCustomizedEntityEndpointTests;

public class PatchWriteOnlyCustomizedEntityEndpointTests {
    private readonly Mock<ICommandDispatcher> _commandDispatcher = new();

    [Theory]
    [InlineData("CustomizedNamePatchManagedEntityEndpoint")]
    [InlineData("CustomizedNamePatchManagedEntityViewModel")]
    public void Should_CustomizeClassNames(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().ContainType(typeName);
    }

    [Fact]
    public async Task Should_ReturnCorrectValue() {
        // Act
        var actual = await CustomizedNamePatchManagedEntityEndpoint
            .RunPatchAsync(
                Guid.NewGuid(),
                new(),
                _commandDispatcher.Object,
                CancellationToken.None
            );

        // Assert
        actual.Should().BeOfType<NoContent>();
    }
}