using System.Collections.Generic;
using ITech.CrudGenerator.Core.Configurations.Configurators;
using ITech.CrudGenerator.Core.Configurations.Crud;
using ITech.CrudGenerator.Core.Configurations.Crud.TypedConfigurations;
using ITech.CrudGenerator.Core.Configurations.Global;
using ITech.CrudGenerator.Core.Configurations.Shared;
using ITech.CrudGenerator.Core.Generators;
using ITech.CrudGenerator.Core.Generators.Core;
using ITech.CrudGenerator.Core.Schemes.DbContext;
using ITech.CrudGenerator.Core.Schemes.Entity;
using ITech.CrudGenerator.Core.Schemes.InternalEntityGenerator.Operations;

namespace ITech.CrudGenerator.Core.Runners;

internal class DeleteCommandGeneratorRunner : IGeneratorRunner
{
    public CqrsOperationWithoutReturnValueGeneratorConfiguration Configuration { get; }
    private readonly EntityScheme _entityScheme;
    private readonly DbContextScheme _dbContextScheme;

    public DeleteCommandGeneratorRunner(
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        InternalEntityGeneratorDeleteOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme,
        DbContextScheme dbContextScheme)
    {
        Configuration = ConstructConfiguration(
            globalConfiguration,
            operationsSharedConfiguration,
            operationConfiguration,
            entityScheme);
        _entityScheme = entityScheme;
        _dbContextScheme = dbContextScheme;
    }

    private static CqrsOperationWithoutReturnValueGeneratorConfiguration ConstructConfiguration(
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        InternalEntityGeneratorDeleteOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme)
    {
        return new CqrsOperationWithoutReturnValueGeneratorConfiguration(
            generate: operationConfiguration?.Generate ?? true,
            globalConfiguration: globalConfiguration,
            operationsSharedConfiguration: operationsSharedConfiguration,
            operationType: CqrsOperationType.Command,
            operationName: operationConfiguration?.Operation ?? "Delete",
            operationGroup: new(operationConfiguration?.OperationGroup ?? "{{operation_name}}{{entity_name}}"),
            operation: new(operationConfiguration?.CommandName ?? "{{operation_name}}{{entity_name}}Command"),
            handler: new(operationConfiguration?.HandlerName ?? "{{operation_name}}{{entity_name}}Handler"),
            endpoint: new MinimalApiEndpointConfigurator
            {
                // If general generate is false, than endpoint generate is also false
                Generate = operationConfiguration?.Generate != false &&
                           (operationConfiguration?.GenerateEndpoint ?? true),
                ClassName = new(operationConfiguration?.EndpointClassName ??
                                "{{operation_name}}{{entity_name}}Endpoint"),
                FunctionName = new(operationConfiguration?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurator = new(operationConfiguration?.RouteName ??
                                                "/{{entity_name}}/{{id_param_name}}/{{operation_name | string.downcase}}")
            },
            entityScheme
        );
    }

    public List<GeneratorResult> RunGenerator(List<EndpointMap> endpointsMaps)
    {
        if (!Configuration.Generate) return [];
        var deleteCommandScheme = new CrudGeneratorScheme<CqrsOperationWithoutReturnValueGeneratorConfiguration>(
            _entityScheme,
            _dbContextScheme,
            Configuration
        );
        var generateDeleteCommand = new DeleteCommandCrudGenerator(deleteCommandScheme);
        generateDeleteCommand.RunGenerator();
        if (generateDeleteCommand.EndpointMap is not null)
        {
            endpointsMaps.Add(generateDeleteCommand.EndpointMap);
        }

        return generateDeleteCommand.GeneratedFiles;
    }
}