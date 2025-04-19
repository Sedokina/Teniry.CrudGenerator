using Microsoft.CodeAnalysis;
using Teniry.CrudGenerator.Core.Configurations.Crud;
using Teniry.CrudGenerator.Core.Configurations.Global;
using Teniry.CrudGenerator.Core.Configurations.Shared;
using Teniry.CrudGenerator.Core.Generators;
using Teniry.CrudGenerator.Core.Generators.Core;
using Teniry.CrudGenerator.Core.Runners;
using Teniry.CrudGenerator.Core.Schemes.Entity;
using Teniry.CrudGenerator.Core.Schemes.InternalEntityGenerator;
using Teniry.CrudGenerator.Tests.Helpers;

namespace Teniry.CrudGenerator.Tests.Generators.Patch;

public class PatchCommandCrudGeneratorTests {
    private readonly CrudGeneratorScheme<CqrsOperationWithoutReturnValueWithReceiveViewModelGeneratorConfiguration>
        _crudGeneratorScheme;

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

        var configuration = new PatchCommandGeneratorRunner(
                globalCqrsGeneratorConfigurationBuilder,
                cqrsOperationsSharedConfigurationBuilder,
                internalEntityGeneratorConfiguration.PatchOperation,
                entityScheme,
                new DbContextSchemeStub()
            )
            .Configuration;
        _crudGeneratorScheme = new(
            entityScheme,
            new DbContextSchemeStub(),
            configuration
        );
    }

    [Theory]
    [InlineData("PatchTestEntityCommand.g.cs")]
    [InlineData("PatchTestEntityHandler.g.cs")]
    [InlineData("PatchTestEntityEndpoint.g.cs")]
    [InlineData("PatchTestEntityVm.g.cs")]
    public Task Should_ReturnGetRouteFromCreateEndpoint(string fileName) {
        // Arrange
        var sut = new PatchCommandCrudGenerator(_crudGeneratorScheme);

        // Act
        sut.RunGenerator();

        // Assert
        var endpoint = sut.GeneratedFiles.Find(x => x.FileName.Equals(fileName));
        endpoint.Should().NotBeNull();

        return Verify(endpoint!.Source.ToString()).UseParameters(fileName);
    }
}