using Teniry.Cqrs.Queries;
using Teniry.CrudGenerator.SampleApi.Application.CustomOperationNameEntityFeature.
    CustomOpGetListCustomOperationNameEntities;
using Teniry.CrudGenerator.SampleApi.Endpoints.CustomOperationNameEntityEndpoints;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace Teniry.CrudGenerator.SampleApiE2eTests.EndpointsTests.CustomOperationNameEntityEndpointTests;

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
                new(),
                _queryDispatcher.Object,
                new()
            );

        // Assert
        actual.Should().BeOfType<Ok<CustomOperationNameEntitiesDto>>();
    }
}