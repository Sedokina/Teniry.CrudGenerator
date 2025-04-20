using Microsoft.CodeAnalysis;
using Teniry.CrudGenerator.Core.Configurations.Global;
using Teniry.CrudGenerator.Core.Configurations.Shared;
using Teniry.CrudGenerator.Core.Generators.Core;
using Teniry.CrudGenerator.Core.Runners;
using Teniry.CrudGenerator.Core.Schemes.Entity;
using Teniry.CrudGenerator.Core.Schemes.InternalEntityGenerator;
using Teniry.CrudGenerator.Core.Schemes.InternalEntityGenerator.Operations;
using Teniry.CrudGenerator.Tests.Helpers;

namespace Teniry.CrudGenerator.Tests.Generators.Patch;

public class PatchCommandCrudGeneratorTests {
    private PatchCommandGeneratorRunner CreateRunner(
        InternalEntityGeneratorPatchOperationConfiguration? patchOperation = null
    ) {
        var globalCqrsGeneratorConfigurationBuilder = GlobalCrudGeneratorConfigurationFactory.Construct();
        var cqrsOperationsSharedConfigurationBuilder = new CqrsOperationsSharedConfiguratorFactory().Construct();
        var internalEntityGeneratorConfiguration = new InternalEntityGeneratorConfiguration(
            new(
                "TestEntity",
                "",
                "",
                [
                    new("Id", "Guid", "Guid", SpecialType.None, true, false),
                    new("Names", "string", "Guid", SpecialType.System_String, true, false)
                ]
            )
        ) {
            PatchOperation = patchOperation
        };
        var entityScheme = EntitySchemeFactory
            .Construct(internalEntityGeneratorConfiguration, new DbContextSchemeStub());

        // TODO: test customization
        return new(
            globalCqrsGeneratorConfigurationBuilder,
            cqrsOperationsSharedConfigurationBuilder,
            internalEntityGeneratorConfiguration.PatchOperation,
            entityScheme,
            new DbContextSchemeStub()
        );
    }

    [Theory]
    [InlineData("PatchTestEntityCommand.g.cs")]
    [InlineData("PatchTestEntityHandler.g.cs")]
    [InlineData("PatchTestEntityEndpoint.g.cs")]
    [InlineData("PatchTestEntityVm.g.cs")]
    public Task Should_ReturnGetRouteFromCreateEndpoint(string fileName) {
        // Arrange
        var sut = CreateRunner();

        // Act
        var generatedFiles = sut.RunGenerator([]);

        // Assert
        var file = generatedFiles.Find(x => x.FileName.Equals(fileName));
        file.Should().NotBeNull();

        return Verify(file!.Source.ToString()).UseParameters(fileName);
    }

    [Fact]
    public Task Should_CorrectlyMapEndpoints() {
        // Arrange
        var sut = CreateRunner();
        List<EndpointMap> endpointsMaps = [];

        // Act
        sut.RunGenerator(endpointsMaps);

        // Assert
        return Verify(endpointsMaps);
    }

    [Fact]
    public void Should_NotGenerateEndpoint_When_GenerateEndpointIsFalse() {
        // Arrange
        var sut = CreateRunner(
            new() {
                GenerateEndpoint = false
            }
        );
        List<EndpointMap> endpointsMaps = [];

        // Act
        var generatedFiles = sut.RunGenerator(endpointsMaps);

        // Assert
        endpointsMaps.Should().BeEmpty();
        generatedFiles.Should().NotContain(x => x.FileName.Contains("Endpoint"));
    }

    [Fact]
    public void Should_GenerateNothing_When_GenerateIsFalse() {
        // Arrange
        var sut = CreateRunner(
            new() {
                Generate = false
            }
        );
        List<EndpointMap> endpointsMaps = [];

        // Act
        var generatedFiles = sut.RunGenerator(endpointsMaps);

        // Assert
        endpointsMaps.Should().BeEmpty();
        generatedFiles.Should().BeEmpty();
    }
}