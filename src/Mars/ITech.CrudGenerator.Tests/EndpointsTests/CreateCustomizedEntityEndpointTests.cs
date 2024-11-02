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
    
    [Theory]
    [InlineData("CustomizedNameCreateManageEntityEndpoint")]
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
        var actual = await CustomizedNameCreateManageEntityEndpoint
            .RunCreateAsync(
                new CustomizedNameCreateManageEntityCommand(),
                _commandDispatcher.Object,
                new CancellationToken());

        // Assert
        actual.Should().BeOfType<Created<CustomizedNameCreatedManageEntityDto>>();
    }
}