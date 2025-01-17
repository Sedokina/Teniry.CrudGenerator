using Teniry.CrudGenerator.Core.Configurations.Configurators;
using Teniry.CrudGenerator.Core.Configurations.Crud;
using Teniry.CrudGenerator.Core.Configurations.Global;
using Teniry.CrudGenerator.Core.Configurations.Shared;
using Teniry.CrudGenerator.Core.Generators;
using Teniry.CrudGenerator.Core.Generators.Core;
using Teniry.CrudGenerator.Core.Runners;
using Teniry.CrudGenerator.Core.Schemes.Entity;
using Teniry.CrudGenerator.Core.Schemes.InternalEntityGenerator;
using Microsoft.CodeAnalysis;
using Teniry.CrudGenerator.Tests.Helpers;

namespace Teniry.CrudGenerator.Tests;

public class CreateCommandCrudGeneratorTests {
    private readonly CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration> _crudGeneratorScheme;

    public CreateCommandCrudGeneratorTests() {
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
                    new("Name", "string", "Guid", SpecialType.System_String, true, false)
                ]
            )
        );
        var entityScheme = EntitySchemeFactory
            .Construct(internalEntityGeneratorConfiguration, new DbContextSchemeStub());

        var configuration = new CreateCommandGeneratorRunner(
                globalCqrsGeneratorConfigurationBuilder,
                cqrsOperationsSharedConfigurationBuilder,
                internalEntityGeneratorConfiguration.CreateOperation,
                entityScheme,
                new DbContextSchemeStub(),
                null!,
                null!
            )
            .Configuration;
        _crudGeneratorScheme = new(
            entityScheme,
            new DbContextSchemeStub(),
            configuration
        );
    }

    [Fact]
    public void Should_ReturnGetRouteFromCreateEndpoint() {
        // Arrange
        var getByIdEndpointRouteConfigurationBuilder =
            new EndpointRouteConfigurator("mygetroute/{{id_param_name}}");
        var sut = new CreateCommandCrudGenerator(
            _crudGeneratorScheme,
            getByIdEndpointRouteConfigurationBuilder,
            "getbyid"
        );

        // Act
        sut.RunGenerator();

        // Assert
        var endpoint = sut.GeneratedFiles.Find(x => x.FileName.Equals("CreateTestEntityEndpoint.g.cs"));
        endpoint.Should().NotBeNull();
        endpoint!.Source.ToString().Should()
            .Contain(@"return TypedResults.Created($""mygetroute/{result.Id}"", result);");
    }

    [Fact]
    public void
        Should_NotReturnGetRouteFromCreateEndpoint_When_GetRouteConfigurationNotProvided_Or_GetEndpointNotGenerated() {
        // Arrange
        var sut = new CreateCommandCrudGenerator(_crudGeneratorScheme, null, null);

        // Act
        sut.RunGenerator();

        // Assert
        var endpoint = sut.GeneratedFiles.Find(x => x.FileName.Equals("CreateTestEntityEndpoint.g.cs"));
        endpoint.Should().NotBeNull();
        endpoint!.Source.ToString().Should()
            .Contain(@"return TypedResults.Created($"""", result);");
    }
}