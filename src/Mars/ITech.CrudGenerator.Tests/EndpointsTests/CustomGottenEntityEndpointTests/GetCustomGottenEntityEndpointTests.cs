using ITech.Cqrs.Cqrs.Queries;
using ITech.CrudGenerator.TestApi.Application.CustomGottenEntityFeature.GetCustomGottenEntities;
using ITech.CrudGenerator.TestApi.Endpoints.CustomGottenEntityEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace ITech.CrudGenerator.Tests.EndpointsTests.CustomGottenEntityEndpointTests;

public class GetCustomGottenEntityEndpointTests
{
    private readonly Mock<IQueryDispatcher> _queryDispatcher = new();

    [Theory]
    [InlineData("GetCustomGottenEntityEndpoint")]
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
        var actual = await GetCustomGottenEntitiesEndpoint
            .GetAsync(new GetCustomGottenEntitiesQuery(),
                _queryDispatcher.Object,
                new CancellationToken());

        // Assert
        actual.Should().BeOfType<Ok<CustomGottenEntitiesDto>>();
    }
}