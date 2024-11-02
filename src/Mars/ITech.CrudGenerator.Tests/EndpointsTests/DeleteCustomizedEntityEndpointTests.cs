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

    [Theory]
    [InlineData("CustomizedNameDeleteManageEntityEndpoint")]
    public async Task Should_CustomizeClassNames(string typeName)
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
        var actual = await CustomizedNameDeleteManageEntityEndpoint
            .RunDeleteAsync(Guid.NewGuid(), 
                _commandDispatcher.Object,
                new CancellationToken());

        // Assert
        actual.Should().BeOfType<NoContent>();
    }
}