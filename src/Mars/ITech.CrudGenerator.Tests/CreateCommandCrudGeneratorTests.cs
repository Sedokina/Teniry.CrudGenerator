using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Configurators;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Crud;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Shared;
using ITech.CrudGenerator.CrudGeneratorCore.GeneratorRunners;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;
using ITech.CrudGenerator.Tests.Helpers;
using Microsoft.CodeAnalysis;

namespace ITech.CrudGenerator.Tests;

public class CreateCommandCrudGeneratorTests
{
    private readonly CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration> _crudGeneratorScheme;

    public CreateCommandCrudGeneratorTests()
    {
        var globalCqrsGeneratorConfigurationBuilder =
            GlobalCrudGeneratorConfigurationFactory.Construct();
        var cqrsOperationsSharedConfigurationBuilder =
            new CqrsOperationsSharedConfiguratorFactory().Construct();
        var internalEntityGeneratorConfiguration = new InternalEntityGeneratorConfiguration(
            new InternalEntityClassMetadata("TestEntity", "", "",
            [
                new("Id", "Guid", "Guid", SpecialType.None, true, false),
                new("Name", "string", "Guid", SpecialType.System_String, true, false)
            ])
        );
        var entityScheme = EntitySchemeFactory
            .Construct(internalEntityGeneratorConfiguration, new DbContextSchemeStub());

        var configuration = new CreateCommandGeneratorRunner(globalCqrsGeneratorConfigurationBuilder,
                cqrsOperationsSharedConfigurationBuilder,
                internalEntityGeneratorConfiguration.CreateOperation,
                entityScheme,
                new DbContextSchemeStub(),
                null!,
                null!)
            .Configuration;
        _crudGeneratorScheme = new CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration>(entityScheme,
            new DbContextSchemeStub(),
            configuration);
    }

    [Fact]
    public void Should_ReturnGetRouteFromCreateEndpoint()
    {
        // Arrange
        var getByIdEndpointRouteConfigurationBuilder =
            new EndpointRouteConfigurator("mygetroute/{{id_param_name}}");
        var sut = new CreateCommandCrudGenerator(_crudGeneratorScheme, getByIdEndpointRouteConfigurationBuilder,
            "getbyid");

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
        Should_NotReturnGetRouteFromCreateEndpoint_When_GetRouteConfigurationNotProvided_Or_GetEndpointNotGenerated()
    {
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