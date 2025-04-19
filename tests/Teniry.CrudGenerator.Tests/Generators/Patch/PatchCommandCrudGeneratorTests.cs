using Microsoft.CodeAnalysis;
using Teniry.CrudGenerator.Core.Configurations.Global;
using Teniry.CrudGenerator.Core.Configurations.Shared;
using Teniry.CrudGenerator.Core.Runners;
using Teniry.CrudGenerator.Core.Schemes.Entity;
using Teniry.CrudGenerator.Core.Schemes.InternalEntityGenerator;
using Teniry.CrudGenerator.Tests.Helpers;

namespace Teniry.CrudGenerator.Tests.Generators.Patch;

public class PatchCommandCrudGeneratorTests {
    private readonly PatchCommandGeneratorRunner _sut;

    public PatchCommandCrudGeneratorTests() {
        var globalCqrsGeneratorConfigurationBuilder =
            GlobalCrudGeneratorConfigurationFactory.Construct();
        var cqrsOperationsSharedConfigurationBuilder =
            new CqrsOperationsSharedConfiguratorFactory().Construct();
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
        );
        var entityScheme = EntitySchemeFactory
            .Construct(internalEntityGeneratorConfiguration, new DbContextSchemeStub());

        _sut = new(
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
        // Act
        var generatedFiles = _sut.RunGenerator([]);

        // Assert
        var file = generatedFiles.Find(x => x.FileName.Equals(fileName));
        file.Should().NotBeNull();

        return Verify(file!.Source.ToString()).UseParameters(fileName);
    }
}