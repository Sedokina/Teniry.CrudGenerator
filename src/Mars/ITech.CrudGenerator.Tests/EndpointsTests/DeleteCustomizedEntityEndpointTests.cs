using ITech.Cqrs.Cqrs.Commands;
using ITech.CrudGenerator.TestApi.Endpoints.CustomizedManageEntityEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace ITech.CrudGenerator.Tests.EndpointsTests;

public class DeleteCustomizedEntityEndpointTests
{
    private readonly Mock<ICommandDispatcher> _commandDispatcher;

    public DeleteCustomizedEntityEndpointTests()
    {
        _commandDispatcher = new();
    }

    [Fact]
    public async Task Should_ReturnCorrectValue()
    {
        // Act
        var actual = await CustomizedNameDeleteManageEntityEndpoint
            .RunDeleteAsync(Guid.NewGuid(), 
                _commandDispatcher.Object,
                new CancellationToken());

        // Assert
        actual.Should().BeOfType<NoContent>();
    }
}