using ITech.Cqrs.Cqrs.Commands;
using ITech.CrudGenerator.TestApi.Endpoints.CustomManagedEntityEndpoints;
using ITech.CrudGenerator.Tests.E2eTests.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace ITech.CrudGenerator.Tests.EndpointsTests.CustomManagedEntityEndpointTests;

public class DeleteCustomManagedEntityEndpointTests
{
    private readonly Mock<ICommandDispatcher> _commandDispatcher = new();

    [Theory]
    [InlineData("CustomizedNameDeleteManagedEntityEndpoint")]
    public void Should_CustomizeClassNames(string typeName)
    {
        // Assert
        typeof(Program).Assembly.Should().ContainType(typeName);
    }
    
    [Fact]
    public async Task Should_ReturnCorrectValue()
    {
        // Act
        var actual = await CustomizedNameDeleteManagedEntityEndpoint
            .RunDeleteAsync(Guid.NewGuid(), 
                _commandDispatcher.Object,
                new CancellationToken());

        // Assert
        actual.Should().BeOfType<NoContent>();
    }
}