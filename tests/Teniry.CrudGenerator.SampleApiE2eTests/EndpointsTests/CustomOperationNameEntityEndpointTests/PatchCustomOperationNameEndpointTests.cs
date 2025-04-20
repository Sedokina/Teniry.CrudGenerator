using Teniry.Cqrs.Commands;
using Teniry.CrudGenerator.SampleApi.Endpoints.CustomOperationNameEntityEndpoints;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace Teniry.CrudGenerator.SampleApiE2eTests.EndpointsTests.CustomOperationNameEntityEndpointTests;

public class PatchCustomOperationNameEndpointTests {
    private readonly Mock<ICommandDispatcher> _commandDispatcher = new();

    [Theory]
    [InlineData("CustomOpPatchCustomOperationNameEntityEndpoint")]
    [InlineData("CustomOpPatchCustomOperationNameEntityVm")]
    public void Should_CustomizeClassNames(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().ContainType(typeName);
    }

    [Fact]
    public async Task Should_ReturnCorrectValue() {
        // Act
        var actual = await CustomOpPatchCustomOperationNameEntityEndpoint
            .CustomOpPatchAsync(
                Guid.NewGuid(),
                new(),
                _commandDispatcher.Object,
                CancellationToken.None
            );

        // Assert
        actual.Should().BeOfType<NoContent>();
    }
}