using ITech.Cqrs.Cqrs.Commands;
using ITech.CrudGenerator.TestApi.Application.CustomOperationNameEntityFeature.CustomOpCreateCustomOperationNameEntity;
using ITech.CrudGenerator.TestApi.Endpoints.CustomOperationNameEntityEndpoints;
using ITech.CrudGenerator.Tests.E2eTests.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace ITech.CrudGenerator.Tests.EndpointsTests.CustomOperationNameEntityEndpointTests;

public class CreateCustomOperationNameEntityEndpointTests
{
    private readonly Mock<ICommandDispatcher> _commandDispatcher = new();

    [Theory]
    [InlineData("CustomOpCreateCustomOperationNameEntityEndpoint")]
    public void Should_CustomizeClassNames(string typeName)
    {
        // Assert
        typeof(Program).Assembly.Should().ContainType(typeName);
    }

    [Fact]
    public async Task Should_ReturnCorrectValue()
    {
        // Arrange
        _commandDispatcher.Setup(x =>
                x.DispatchAsync<CustomOpCreateCustomOperationNameEntityCommand, CreatedCustomOperationNameEntityDto>(
                    It.IsAny<CustomOpCreateCustomOperationNameEntityCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreatedCustomOperationNameEntityDto(Guid.NewGuid()));

        // Act
        var actual = await CustomOpCreateCustomOperationNameEntityEndpoint
            .CustomOpCreateAsync(
                new CustomOpCreateCustomOperationNameEntityCommand(),
                _commandDispatcher.Object,
                new CancellationToken());

        // Assert
        actual.Should().BeOfType<Created<CreatedCustomOperationNameEntityDto>>()
            .Subject.Location.Should().NotBeEmpty();
    }
}