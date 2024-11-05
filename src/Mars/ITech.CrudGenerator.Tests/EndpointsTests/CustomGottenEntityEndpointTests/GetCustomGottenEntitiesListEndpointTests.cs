using ITech.Cqrs.Cqrs.Queries;
using ITech.CrudGenerator.TestApi.Application.CustomGottenEntityFeature.CustomGottenEntityGetListOperationCustomNs;
using ITech.CrudGenerator.TestApi.Endpoints.CustomGottenEntityEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace ITech.CrudGenerator.Tests.EndpointsTests.CustomGottenEntityEndpointTests;

public class GetCustomGottenEntitiesListEndpointTests
{
    private readonly Mock<IQueryDispatcher> _queryDispatcher = new();

    [Theory]
    [InlineData("CustomizedNameGetCustomEntitiesListEndpoint")]
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
    public async Task Should_ReturnCorrectResult()
    {
        // Act
        var actual = await CustomizedNameGetCustomEntitiesListEndpoint
            .RunGetListAsync(new CustomizedNameGetCustomEntitiesListQuery(),
                _queryDispatcher.Object,
                new CancellationToken());

        // Assert
        actual.Should().BeOfType<Ok<CustomizedNameGetCustomEntitiesListDto>>();
    }
}