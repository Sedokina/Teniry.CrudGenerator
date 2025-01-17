using ITech.Cqrs.Cqrs.Commands;
using Teniry.CrudGenerator.SampleApi.Application.CustomManagedEntityFeature.ManagedEntityCreateOperationCustomNs;
using Teniry.CrudGenerator.SampleApi.Endpoints.CustomManagedEntityEndpoints;
using Teniry.CrudGenerator.TestApiTests.E2eTests.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace Teniry.CrudGenerator.TestApiTests.EndpointsTests.CustomManagedEntityEndpointTests;

public class CreateCustomManagedEntityEndpointTests {
    private readonly Mock<ICommandDispatcher> _commandDispatcher = new();

    [Theory]
    [InlineData("CustomizedNameCreateManagedEntityEndpoint")]
    public void Should_CustomizeClassNames(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().ContainType(typeName);
    }

    [Fact]
    public async Task Should_ReturnCorrectValue() {
        // Act
        var actual = await CustomizedNameCreateManagedEntityEndpoint
            .RunCreateAsync(
                new(),
                _commandDispatcher.Object,
                new()
            );

        // Assert
        actual.Should().BeOfType<Created<CustomizedNameCreatedManagedEntityDto>>()
            .Subject.Location.Should().BeNullOrEmpty("because get endpoint is not generated for this entity");
    }
}