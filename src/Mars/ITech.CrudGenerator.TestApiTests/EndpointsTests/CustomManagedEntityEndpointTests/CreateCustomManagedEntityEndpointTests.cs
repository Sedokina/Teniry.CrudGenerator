using ITech.Cqrs.Cqrs.Commands;
using ITech.CrudGenerator.TestApi.Application.CustomManagedEntityFeature.ManagedEntityCreateOperationCustomNs;
using ITech.CrudGenerator.TestApi.Endpoints.CustomManagedEntityEndpoints;
using ITech.CrudGenerator.TestApiTests.E2eTests.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace ITech.CrudGenerator.TestApiTests.EndpointsTests.CustomManagedEntityEndpointTests;

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