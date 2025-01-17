using ITech.Cqrs.Cqrs.Queries;
using ITech.CrudGenerator.TestApi.Application.CustomGottenEntityFeature.CustomGottenEntityGetOperationCustomNs;
using ITech.CrudGenerator.TestApi.Endpoints.CustomGottenEntityEndpoints;
using ITech.CrudGenerator.TestApiTests.E2eTests.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace ITech.CrudGenerator.TestApiTests.EndpointsTests.CustomGottenEntityEndpointTests;

public class GetCustomGottenEntityEndpointTests {
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