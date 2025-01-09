using System.Collections.Generic;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

internal class CreateCommandDefaultConfigurationBuilderFactory : IConfigurationBuilderFactory
{
    private readonly CqrsOperationWithReturnValueConfigurationBuilder _builder;
    private readonly EntityScheme _entityScheme;
    private readonly DbContextScheme _dbContextScheme;
    private CqrsOperationWithReturnValueGeneratorConfiguration _getByIdQueryConfiguration;
    private CqrsOperationWithReturnValueConfigurationBuilder _getByIdQueryConfigurationBuilder;

    public CreateCommandDefaultConfigurationBuilderFactory(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        InternalEntityGeneratorCreateOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme,
        DbContextScheme dbContextScheme)
    {
        _builder = ConstructBuilder(globalConfiguration, operationsSharedConfiguration, operationConfiguration);
        _entityScheme = entityScheme;
        _dbContextScheme = dbContextScheme;
    }
    
    public CqrsOperationWithReturnValueConfigurationBuilder ConstructBuilder(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        InternalEntityGeneratorCreateOperationConfiguration? operationConfiguration)
    {
        return new CqrsOperationWithReturnValueConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            Generate = operationConfiguration?.Generate ?? true,
            OperationType = CqrsOperationType.Command,
            OperationName = operationConfiguration?.Operation ?? "Create",
            OperationGroup = new(operationConfiguration?.OperationGroup ?? "{{operation_name}}{{entity_name}}"),
            Operation = new(operationConfiguration?.CommandName ?? "{{operation_name}}{{entity_name}}Command"),
            Dto = new(operationConfiguration?.DtoName ?? "Created{{entity_name}}Dto"),
            Handler = new(operationConfiguration?.HandlerName ?? "{{operation_name}}{{entity_name}}Handler"),
            Endpoint = new()
            {
                // If general generate is false, than endpoint generate is also false
                Generate = operationConfiguration?.Generate != false &&
                           (operationConfiguration?.GenerateEndpoint ?? true),
                ClassName = new(operationConfiguration?.EndpointClassName ??
                                "{{operation_name}}{{entity_name}}Endpoint"),
                FunctionName = new(operationConfiguration?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurationBuilder = new(operationConfiguration?.RouteName ??
                                                "/{{entity_name}}/{{operation_name | string.downcase}}")
            }
        };
    }

    public List<GeneratorResult> RunGenerator(List<EndpointMap> endpointsMaps)
    {
        var createCommandConfiguration = _builder.Build(_entityScheme);
        if (!createCommandConfiguration.Generate) return [];
        var createCommandScheme =
            new CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration>(
                _entityScheme,
                _dbContextScheme,
                createCommandConfiguration
            );
        var generateCreateCommand = new CreateCommandCrudGenerator(
            createCommandScheme,
            _getByIdQueryConfiguration.Endpoint.Generate
                ? _getByIdQueryConfigurationBuilder.Endpoint.RouteConfigurationBuilder
                : null,
            _getByIdQueryConfiguration.Endpoint.Generate
                ? _getByIdQueryConfigurationBuilder.OperationName
                : null);
        generateCreateCommand.RunGenerator();
        if (generateCreateCommand.EndpointMap is not null)
        {
            endpointsMaps.Add(generateCreateCommand.EndpointMap);
        }

        return generateCreateCommand.GeneratedFiles;
    }
}