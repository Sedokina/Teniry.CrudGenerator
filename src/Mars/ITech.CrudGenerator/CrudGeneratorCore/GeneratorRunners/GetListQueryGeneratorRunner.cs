using System.Collections.Generic;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;

namespace ITech.CrudGenerator.CrudGeneratorCore.GeneratorRunners;

internal class GetListQueryGeneratorRunner : IGeneratorRunner
{
    public CqrsListOperationGeneratorConfiguration Configuration { get; }
    private readonly EntityScheme _entityScheme;
    private readonly DbContextScheme _dbContextScheme;

    public GetListQueryGeneratorRunner(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        InternalEntityGeneratorGetListOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme,
        DbContextScheme dbContextScheme)
    {
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
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        InternalEntityGeneratorGetListOperationConfiguration? operationConfiguration,
        EntityScheme entityScheme)
    {
        return new CqrsListOperationGeneratorConfiguration(generate: operationConfiguration?.Generate ?? true,
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
            endpoint: new MinimalApiEndpointConfigurationBuilder
            {
                Generate = operationConfiguration?.Generate != false &&
                           (operationConfiguration?.GenerateEndpoint ?? true),
                ClassName = new(operationConfiguration?.EndpointClassName ??
                                "{{operation_name}}{{entity_name_plural}}Endpoint"),
                FunctionName = new(operationConfiguration?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurationBuilder = new(operationConfiguration?.RouteName ?? "/{{entity_name}}")
            },
            entityScheme: entityScheme
        );
    }

    public List<GeneratorResult> RunGenerator(List<EndpointMap> endpointsMaps)
    {
        if (Configuration.Generate)
        {
            var getListQueryScheme = new CrudGeneratorScheme<CqrsListOperationGeneratorConfiguration>(
                _entityScheme,
                _dbContextScheme,
                Configuration);
            var generateListQuery = new ListQueryCrudGenerator(getListQueryScheme);
            generateListQuery.RunGenerator();
            if (generateListQuery.EndpointMap is not null)
            {
                endpointsMaps.Add(generateListQuery.EndpointMap);
            }

            return generateListQuery.GeneratedFiles;
        }

        return [];
    }
}