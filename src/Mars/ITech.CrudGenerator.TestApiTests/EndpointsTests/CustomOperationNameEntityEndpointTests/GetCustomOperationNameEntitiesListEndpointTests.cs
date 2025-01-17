using ITech.Cqrs.Cqrs.Queries;
using ITech.CrudGenerator.TestApi.Application.CustomOperationNameEntityFeature.
    CustomOpGetListCustomOperationNameEntities;
using ITech.CrudGenerator.TestApi.Endpoints.CustomOperationNameEntityEndpoints;
using ITech.CrudGenerator.TestApiTests.E2eTests.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace ITech.CrudGenerator.TestApiTests.EndpointsTests.CustomOperationNameEntityEndpointTests;

public class GetCustomOperationNameEntitiesListEndpointTests {
    private readonly Mock<IQueryDispatcher> _queryDispatcher = new();

    [Theory]
    [InlineData("CustomOpGetListCustomOperationNameEntitiesEndpoint")]
    public void Should_CustomizeClassNames(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().ContainType(typeName);
    }

    [Fact]
    public async Task Should_ReturnCorrectResult() {
        // Act`
        var actual = await CustomOpGetListCustomOperationNameEntitiesEndpoint
            .CustomOpGetListAsync(
                new CustomOpGetListCustomOperationNameEntitiesQuery(),
                _queryDispatcher.Object,
                new CancellationToken()
            );

        // Assert
        actual.Should().BeOfType<Ok<CustomOperationNameEntitiesDto>>();
    }
}