using ITech.Cqrs.Cqrs.Commands;
using ITech.CrudGenerator.TestApi.Endpoints.CustomManagedEntityEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace ITech.CrudGenerator.Tests.EndpointsTests;

public class UpdateCustomManagedEntityEndpointTests
{
    private readonly Mock<ICommandDispatcher> _commandDispatcher = new();

    [Theory]
    [InlineData("CustomizedNameUpdateManagedEntityEndpoint")]
    [InlineData("CustomizedNameUpdateManagedEntityViewModel")]
    public void Should_CustomizeClassNames(string typeName)
    {
        // Act
        var foundTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.Name.Equals(typeName));
        
        // Assert
        foundTypes.Should().NotBeEmpty();
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