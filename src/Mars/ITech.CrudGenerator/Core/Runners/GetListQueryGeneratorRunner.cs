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

internal record GetListQueryGeneratorRunner : IGeneratorRunner {
    public CqrsListOperationGeneratorConfiguration Configuration { get; }
    private readonly EntityScheme _entityScheme;
    private readonly DbContextScheme _dbContextScheme;

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

    private static CqrsListOperationGeneratorConfiguration ConstructConfiguration(
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        InternalEntityGeneratorGetListOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme
    ) {
        return new CqrsListOperationGeneratorConfiguration(
            generate: operationConfiguration?.Generate ?? true,
            globalConfiguration: globalConfiguration,
            operationsSharedConfiguration: operationsSharedConfiguration,
            operationType: CqrsOperationType.Query,
            operationName: operationConfiguration?.Operation ?? "Get",
            operationGroup: new(operationConfiguration?.OperationGroup ?? "{{operation_name}}{{entity_name_plural}}"),
            operation: new(operationConfiguration?.QueryName ?? "{{operation_name}}{{entity_name_plural}}Query"),
            dto: new(operationConfiguration?.DtoName ?? "{{entity_name_plural}}Dto"),
            dtoListItem: new(operationConfiguration?.ListItemDtoName ?? "{{entity_name_plural}}ListItemDto"),
            filter: new(operationConfiguration?.FilterName ?? "{{operation_name}}{{entity_name_plural}}Filter"),
            handler: new(operationConfiguration?.HandlerName ?? "{{operation_name}}{{entity_name_plural}}Handler"),
            endpoint: new MinimalApiEndpointConfigurator {
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
}