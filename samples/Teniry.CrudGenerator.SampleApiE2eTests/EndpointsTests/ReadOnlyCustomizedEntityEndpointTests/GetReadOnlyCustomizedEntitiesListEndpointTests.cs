using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Teniry.Cqrs.Queries;
using Teniry.CrudGenerator.SampleApi.Application.ReadOnlyCustomizedEntityFeature.GetReadOnlyModelsListCustomNamespace;
using Teniry.CrudGenerator.SampleApi.Endpoints.ReadOnlyCustomizedEntityEndpoints;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.EndpointsTests.ReadOnlyCustomizedEntityEndpointTests;

public class GetReadOnlyCustomizedEntitiesListEndpointTests {
    private readonly Mock<IQueryDispatcher> _queryDispatcher = new();

    [Theory]
    [InlineData("ReadOnlyModelsListCustomizedEndpoint")]
    public void Should_CustomizeClassNames(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().ContainType(typeName);
    }

    [Fact]
    public async Task Should_ReturnCorrectResult() {
        // Act
        var actual = await ReadOnlyModelsListCustomizedEndpoint
            .RunGetListAsync(
                new(),
                _queryDispatcher.Object,
                new()
            );

        // Assert
        actual.Should().BeOfType<Ok<ReadOnlyModelsListCustomDto>>();
    }
}