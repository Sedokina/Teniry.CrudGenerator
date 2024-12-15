using ITech.Cqrs.Cqrs.Commands;
using ITech.CrudGenerator.TestApi.Endpoints.CustomManagedEntityEndpoints;
using ITech.CrudGenerator.TestApiTests.E2eTests.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace ITech.CrudGenerator.TestApiTests.EndpointsTests.CustomManagedEntityEndpointTests;

public class UpdateCustomManagedEntityEndpointTests
{
    private readonly Mock<ICommandDispatcher> _commandDispatcher = new();

    [Theory]
    [InlineData("CustomizedNameUpdateManagedEntityEndpoint")]
    [InlineData("CustomizedNameUpdateManagedEntityViewModel")]
    public void Should_CustomizeClassNames(string typeName)
    {
        // Assert
        typeof(Program).Assembly.Should().ContainType(typeName);
    }
    
    [Fact]
    public async Task Should_ReturnCorrectValue()
    {
        // Act
        var actual = await CustomizedNameUpdateManagedEntityEndpoint
            .RunUpdateAsync(Guid.NewGuid(),
                new CustomizedNameUpdateManagedEntityViewModel(),
                _commandDispatcher.Object,
                new CancellationToken());

        // Assert
        actual.Should().BeOfType<NoContent>();
    }
}