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

internal record PatchCommandGeneratorRunner : IGeneratorRunner {
    private readonly DbContextScheme _dbContextScheme;
    private readonly EntityScheme _entityScheme;
    public CqrsOperationWithoutReturnValueWithReceiveViewModelGeneratorConfiguration Configuration { get; }

    public PatchCommandGeneratorRunner(
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        InternalEntityGeneratorPatchOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme,
        DbContextScheme dbContextScheme
    ) {
        Configuration = ConstructBuilder(
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

        var patchCommandScheme =
            new CrudGeneratorScheme<CqrsOperationWithoutReturnValueWithReceiveViewModelGeneratorConfiguration>(
                _entityScheme,
                _dbContextScheme,
                Configuration
            );
        var generateUpdateCommand = new PatchCommandCrudGenerator(patchCommandScheme);
        generateUpdateCommand.RunGenerator();
        if (generateUpdateCommand.EndpointMap is not null) {
            endpointsMaps.Add(generateUpdateCommand.EndpointMap);
        }

        return generateUpdateCommand.GeneratedFiles;
    }

    private static CqrsOperationWithoutReturnValueWithReceiveViewModelGeneratorConfiguration ConstructBuilder(
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        InternalEntityGeneratorPatchOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme
    ) {
        return new(
            operationConfiguration?.Generate ?? true,
            globalConfiguration,
            operationsSharedConfiguration,
            CqrsOperationType.Command,
            operationConfiguration?.Operation ?? "Patch",
            new(operationConfiguration?.OperationGroup ?? "{{operation_name}}{{entity_name}}"),
            new(operationConfiguration?.CommandName ?? "{{operation_name}}{{entity_name}}Command"),
            new(operationConfiguration?.HandlerName ?? "{{operation_name}}{{entity_name}}Handler"),
            new(operationConfiguration?.ViewModelName ?? "{{operation_name}}{{entity_name}}Vm"),
            new() {
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