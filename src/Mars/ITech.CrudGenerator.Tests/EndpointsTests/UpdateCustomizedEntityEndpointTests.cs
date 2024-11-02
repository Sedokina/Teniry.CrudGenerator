using ITech.Cqrs.Cqrs.Commands;
using ITech.CrudGenerator.TestApi.Endpoints.CustomizedManageEntityEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace ITech.CrudGenerator.Tests.EndpointsTests;

public class UpdateCustomizedEntityEndpointTests
{
    private readonly Mock<ICommandDispatcher> _commandDispatcher;

    public UpdateCustomizedEntityEndpointTests()
    {
        _commandDispatcher = new();
    }
    
    [Theory]
    [InlineData("CustomizedNameUpdateManageEntityEndpoint")]
    [InlineData("UpdateCustomizedManageEntityVm")]
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
        var actual = await CustomizedNameUpdateManageEntityEndpoint
            .RunUpdateAsync(Guid.NewGuid(),
                new UpdateCustomizedManageEntityVm(),
                _commandDispatcher.Object,
                new CancellationToken());

        // Assert
        actual.Should().BeOfType<NoContent>();
    }
}