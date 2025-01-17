using System.Collections.Generic;
using Teniry.CrudGenerator.Core.Configurations.Crud;
using Teniry.CrudGenerator.Core.Configurations.Crud.TypedConfigurations;
using Teniry.CrudGenerator.Core.Configurations.Global;
using Teniry.CrudGenerator.Core.Configurations.Shared;
using Teniry.CrudGenerator.Core.Generators;
using Teniry.CrudGenerator.Core.Generators.Core;
using Teniry.CrudGenerator.Core.Schemes.DbContext;
using Teniry.CrudGenerator.Core.Schemes.Entity;
using Teniry.CrudGenerator.Core.Schemes.InternalEntityGenerator.Operations;

namespace Teniry.CrudGenerator.Core.Runners;

internal record DeleteCommandGeneratorRunner : IGeneratorRunner {
    private readonly DbContextScheme _dbContextScheme;
    private readonly EntityScheme _entityScheme;
    public CqrsOperationWithoutReturnValueGeneratorConfiguration Configuration { get; }

    public DeleteCommandGeneratorRunner(
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        InternalEntityGeneratorDeleteOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme,
        DbContextScheme dbContextScheme
    ) {
        Configuration = ConstructConfiguration(
            globalConfiguration,
            operationsSharedConfiguration,
            operationConfiguration,
            entityScheme
        );
        _entityScheme = entityScheme;
        _dbContextScheme = dbContextScheme;
    }

    public List<GeneratorResult> RunGenerator(List<EndpointMap> endpointsMaps) {
        if (!Configuration.Generate) return [];

        var deleteCommandScheme = new CrudGeneratorScheme<CqrsOperationWithoutReturnValueGeneratorConfiguration>(
            _entityScheme,
            _dbContextScheme,
            Configuration
        );
        var generateDeleteCommand = new DeleteCommandCrudGenerator(deleteCommandScheme);
        generateDeleteCommand.RunGenerator();
        if (generateDeleteCommand.EndpointMap is not null) {
            endpointsMaps.Add(generateDeleteCommand.EndpointMap);
        }

        return generateDeleteCommand.GeneratedFiles;
    }

    private static CqrsOperationWithoutReturnValueGeneratorConfiguration ConstructConfiguration(
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        InternalEntityGeneratorDeleteOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme
    ) {
        return new(
            operationConfiguration?.Generate ?? true,
            globalConfiguration,
            operationsSharedConfiguration,
            CqrsOperationType.Command,
            operationConfiguration?.Operation ?? "Delete",
            new(operationConfiguration?.OperationGroup ?? "{{operation_name}}{{entity_name}}"),
            new(operationConfiguration?.CommandName ?? "{{operation_name}}{{entity_name}}Command"),
            new(operationConfiguration?.HandlerName ?? "{{operation_name}}{{entity_name}}Handler"),
            new() {
                // If general generate is false, than endpoint generate is also false
                Generate = operationConfiguration?.Generate != false &&
                    (operationConfiguration?.GenerateEndpoint ?? true),
                ClassName = new(
                    operationConfiguration?.EndpointClassName ??
                    "{{operation_name}}{{entity_name}}Endpoint"
                ),
                FunctionName = new(operationConfiguration?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurator = new(
                    operationConfiguration?.RouteName ??
                    "/{{entity_name}}/{{id_param_name}}/{{operation_name | string.downcase}}"
                )
            },
            entityScheme
        );
    }
}