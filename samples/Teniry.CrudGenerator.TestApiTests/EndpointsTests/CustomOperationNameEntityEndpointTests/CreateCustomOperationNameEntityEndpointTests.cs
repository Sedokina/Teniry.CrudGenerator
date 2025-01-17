using ITech.Cqrs.Cqrs.Commands;
using Teniry.CrudGenerator.SampleApi.Application.CustomOperationNameEntityFeature.CustomOpCreateCustomOperationNameEntity;
using Teniry.CrudGenerator.SampleApi.Endpoints.CustomOperationNameEntityEndpoints;
using Teniry.CrudGenerator.TestApiTests.E2eTests.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace Teniry.CrudGenerator.TestApiTests.EndpointsTests.CustomOperationNameEntityEndpointTests;

public class CreateCustomOperationNameEntityEndpointTests {
    private readonly Mock<ICommandDispatcher> _commandDispatcher = new();

    [Theory]
    [InlineData("CustomOpCreateCustomOperationNameEntityEndpoint")]
    public void Should_CustomizeClassNames(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().ContainType(typeName);
    }

    [Fact]
    public async Task Should_ReturnCorrectValue() {
        // Arrange
        _commandDispatcher.Setup(
                x =>
                    x.DispatchAsync<CustomOpCreateCustomOperationNameEntityCommand,
                        CreatedCustomOperationNameEntityDto>(
                        It.IsAny<CustomOpCreateCustomOperationNameEntityCommand>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(new CreatedCustomOperationNameEntityDto(Guid.NewGuid()));

        // Act
        var actual = await CustomOpCreateCustomOperationNameEntityEndpoint
            .CustomOpCreateAsync(
                new(),
                _commandDispatcher.Object,
                new()
            );

        // Assert
        actual.Should().BeOfType<Created<CreatedCustomOperationNameEntityDto>>()
            .Subject.Location.Should().NotBeEmpty();
    }
}