using Teniry.Cqrs.Queries;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Teniry.CrudGenerator.SampleApi.Application.ReadOnlyCustomizedEntityFeature.CustomGottenEntityGetOperationCustomNs;
using Teniry.CrudGenerator.SampleApi.Endpoints.ReadOnlyCustomizedEntityEndpoints;

namespace Teniry.CrudGenerator.SampleApiE2eTests.EndpointsTests.CustomGottenEntityEndpointTests;

public class GetReadOnlyCustomizedEntityEndpointTests {
    private readonly Mock<IQueryDispatcher> _queryDispatcher = new();

    [Theory]
    [InlineData("CustomizedNameGetCustomEntityEndpoint")]
    public void Should_CustomizeClassNames(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().ContainType(typeName);
    }

    [Fact]
    public async Task Should_ReturnCorrectResult() {
        // Act
        var actual = await CustomizedNameGetCustomEntityEndpoint
            .RunGetAsync(
                Guid.NewGuid(),
                _queryDispatcher.Object,
                new()
            );

        // Assert
        actual.Should().BeOfType<Ok<CustomizedNameGetCustomEntityDto>>();
    }
}