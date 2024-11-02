using ITech.Cqrs.Cqrs.Commands;
using ITech.CrudGenerator.TestApi.Application.CustomizedManageEntityFeature.CreateCustomizedManageEntity;
using ITech.CrudGenerator.TestApi.Endpoints.CustomizedManageEntityEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace ITech.CrudGenerator.Tests.EndpointsTests;

public class CreateCustomizedEntityEndpointTests
{
    private readonly Mock<ICommandDispatcher> _commandDispatcher;

    public CreateCustomizedEntityEndpointTests()
    {
        _commandDispatcher = new();
    }

    [Fact]
    public async Task Should_ReturnCorrectValue()
    {
        // Act
        var actual = await CustomizedNameCreateManageEntityEndpoint
            .RunCreateAsync(
                new CustomizedNameCreateManageEntityCommand(),
                _commandDispatcher.Object,
                new CancellationToken());

        // Assert
        actual.Should().BeOfType<Created<CustomizedNameCreatedManageEntityDto>>();
    }
}