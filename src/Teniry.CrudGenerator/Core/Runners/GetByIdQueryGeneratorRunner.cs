using System.Collections.Generic;
using Teniry.CrudGenerator.Core.Configurations.Configurators;
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

internal record GetByIdQueryGeneratorRunner : IGeneratorRunner {
    private readonly DbContextScheme _dbContextScheme;
    private readonly EntityScheme _entityScheme;
    public CqrsOperationWithReturnValueGeneratorConfiguration Configuration { get; }

    public GetByIdQueryGeneratorRunner(
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        InternalEntityGeneratorGetByIdOperationConfiguration? operationConfiguration,
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

        var getByIdQueryScheme = new CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration>(
            _entityScheme,
            _dbContextScheme,
            Configuration
        );
        var generateGetByIdQuery = new GetByIdQueryCrudGenerator(getByIdQueryScheme);
        generateGetByIdQuery.RunGenerator();
        if (generateGetByIdQuery.EndpointMap is not null) {
            endpointsMaps.Add(generateGetByIdQuery.EndpointMap);
        }

        return generateGetByIdQuery.GeneratedFiles;
    }

    private static CqrsOperationWithReturnValueGeneratorConfiguration ConstructConfiguration(
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        InternalEntityGeneratorGetByIdOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme
    ) {
        return new(
            operationConfiguration?.Generate ?? true,
            globalConfiguration,
            operationsSharedConfiguration,
            CqrsOperationType.Query,
            operationConfiguration?.Operation ?? "Get",
            new(operationConfiguration?.OperationGroup ?? "{{operation_name}}{{entity_name}}"),
            new(operationConfiguration?.QueryName ?? "{{operation_name}}{{entity_name}}Query"),
            new(operationConfiguration?.DtoName ?? "{{entity_name}}Dto"),
            new(operationConfiguration?.HandlerName ?? "{{operation_name}}{{entity_name}}Handler"),
            new() {
                Generate = operationConfiguration?.Generate != false &&
                    (operationConfiguration?.GenerateEndpoint ?? true),
                ClassName = new(
                    operationConfiguration?.EndpointClassName ??
                    "{{operation_name}}{{entity_name}}Endpoint"
                ),
                FunctionName = new(operationConfiguration?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurator = GetRouteConfigurationBuilder(operationConfiguration)
            },
            entityScheme
        );
    }

    public static EndpointRouteConfigurator GetRouteConfigurationBuilder(
        InternalEntityGeneratorGetByIdOperationConfiguration? operationConfiguration
    ) {
        return new(operationConfiguration?.RouteName ?? "/{{entity_name}}/{{id_param_name}}");
    }
}