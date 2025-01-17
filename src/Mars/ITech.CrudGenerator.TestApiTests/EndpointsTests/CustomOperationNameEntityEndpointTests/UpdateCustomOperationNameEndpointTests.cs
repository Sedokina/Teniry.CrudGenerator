using ITech.Cqrs.Cqrs.Commands;
using ITech.CrudGenerator.TestApi.Endpoints.CustomOperationNameEntityEndpoints;
using ITech.CrudGenerator.TestApiTests.E2eTests.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace ITech.CrudGenerator.TestApiTests.EndpointsTests.CustomOperationNameEntityEndpointTests;

public class UpdateCustomOperationNameEndpointTests {
    private readonly Mock<ICommandDispatcher> _commandDispatcher = new();

    [Theory]
    [InlineData("CustomOpUpdateCustomOperationNameEntityEndpoint")]
    [InlineData("CustomOpUpdateCustomOperationNameEntityVm")]
    public void Should_CustomizeClassNames(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().ContainType(typeName);
    }

    [Fact]
    public async Task Should_ReturnCorrectValue() {
        // Act
        var actual = await CustomOpUpdateCustomOperationNameEntityEndpoint
            .CustomOpUpdateAsync(
                Guid.NewGuid(),
                new CustomOpUpdateCustomOperationNameEntityVm(),
                _commandDispatcher.Object,
                new CancellationToken()
            );

        // Assert
        actual.Should().BeOfType<NoContent>();
    }
}