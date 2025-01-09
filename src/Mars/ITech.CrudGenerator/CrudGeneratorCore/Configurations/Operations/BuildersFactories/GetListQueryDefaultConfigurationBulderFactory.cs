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

internal class GetListQueryDefaultConfigurationBulderFactory : IConfigurationBuilderFactory
{
    private CqrsListOperationConfigurationBuilder _builder;
    private EntityScheme _entityScheme;
    private DbContextScheme _dbContextScheme;

    public CqrsListOperationConfigurationBuilder Construct(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        InternalEntityGeneratorGetListOperationConfiguration? operationConfiguration)
    {
        return new CqrsListOperationConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            Generate = operationConfiguration?.Generate ?? true,
            OperationType = CqrsOperationType.Query,
            OperationName = operationConfiguration?.Operation ?? "Get",
            OperationGroup = new(operationConfiguration?.OperationGroup ?? "{{operation_name}}{{entity_name_plural}}"),
            Operation = new(operationConfiguration?.QueryName ?? "{{operation_name}}{{entity_name_plural}}Query"),
            Dto = new(operationConfiguration?.DtoName ?? "{{entity_name_plural}}Dto"),
            DtoListItem = new(operationConfiguration?.ListItemDtoName ?? "{{entity_name_plural}}ListItemDto"),
            Filter = new(operationConfiguration?.FilterName ?? "{{operation_name}}{{entity_name_plural}}Filter"),
            Handler = new(operationConfiguration?.HandlerName ?? "{{operation_name}}{{entity_name_plural}}Handler"),
            Endpoint = new()
            {
                // If general generate is false, than endpoint generate is also false
                Generate = operationConfiguration?.Generate != false &&
                           (operationConfiguration?.GenerateEndpoint ?? true),
                ClassName = new(operationConfiguration?.EndpointClassName ??
                                "{{operation_name}}{{entity_name_plural}}Endpoint"),
                FunctionName = new(operationConfiguration?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurationBuilder = new(operationConfiguration?.RouteName ?? "/{{entity_name}}")
            }
        };
    }

    public List<GeneratorResult> RunGenerator(List<EndpointMap> endpointsMaps)
    {
        var getListConfiguration = _builder.Build(_entityScheme);
        if (getListConfiguration.Generate)
        {
            var getListQueryScheme = new CrudGeneratorScheme<CqrsListOperationGeneratorConfiguration>(
                _entityScheme,
                _dbContextScheme,
                getListConfiguration);
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