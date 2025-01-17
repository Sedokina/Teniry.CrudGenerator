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

internal record GetListQueryGeneratorRunner : IGeneratorRunner {
    private readonly DbContextScheme _dbContextScheme;
    private readonly EntityScheme _entityScheme;
    public CqrsListOperationGeneratorConfiguration Configuration { get; }

    public GetListQueryGeneratorRunner(
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        InternalEntityGeneratorGetListOperationConfiguration? operationConfiguration,
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
        if (Configuration.Generate) {
            var getListQueryScheme = new CrudGeneratorScheme<CqrsListOperationGeneratorConfiguration>(
                _entityScheme,
                _dbContextScheme,
                Configuration
            );
            var generateListQuery = new ListQueryCrudGenerator(getListQueryScheme);
            generateListQuery.RunGenerator();
            if (generateListQuery.EndpointMap is not null) {
                endpointsMaps.Add(generateListQuery.EndpointMap);
            }

            return generateListQuery.GeneratedFiles;
        }

        return [];
    }

    private static CqrsListOperationGeneratorConfiguration ConstructConfiguration(
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        InternalEntityGeneratorGetListOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme
    ) {
        return new(
            operationConfiguration?.Generate ?? true,
            globalConfiguration,
            operationsSharedConfiguration,
            CqrsOperationType.Query,
            operationConfiguration?.Operation ?? "Get",
            new(operationConfiguration?.OperationGroup ?? "{{operation_name}}{{entity_name_plural}}"),
            new(operationConfiguration?.QueryName ?? "{{operation_name}}{{entity_name_plural}}Query"),
            new(operationConfiguration?.DtoName ?? "{{entity_name_plural}}Dto"),
            dtoListItem: new(operationConfiguration?.ListItemDtoName ?? "{{entity_name_plural}}ListItemDto"),
            filter: new(operationConfiguration?.FilterName ?? "{{operation_name}}{{entity_name_plural}}Filter"),
            handler: new(operationConfiguration?.HandlerName ?? "{{operation_name}}{{entity_name_plural}}Handler"),
            endpoint: new() {
                Generate = operationConfiguration?.Generate != false &&
                    (operationConfiguration?.GenerateEndpoint ?? true),
                ClassName = new(
                    operationConfiguration?.EndpointClassName ??
                    "{{operation_name}}{{entity_name_plural}}Endpoint"
                ),
                FunctionName = new(operationConfiguration?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurator = new(operationConfiguration?.RouteName ?? "/{{entity_name}}")
            },
            entityScheme: entityScheme
        );
    }
}