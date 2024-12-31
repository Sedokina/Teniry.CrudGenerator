using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.Operations;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

internal class GetListQueryDefaultConfigurationBulderFactory
{
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
            Operation = new()
            {
                TemplatePath = new("{{templates_base_path}}.GetList.GetListQuery.txt"),
                NameConfigurationBuilder = new(operationConfiguration?.QueryName ??
                                               "{{operation_name}}{{entity_name_plural}}Query")
            },
            Dto = new()
            {
                TemplatePath = new("{{templates_base_path}}.GetList.GetListDto.txt"),
                NameConfigurationBuilder = new(operationConfiguration?.DtoName ??
                                               "{{entity_name_plural}}Dto")
            },
            DtoListItem = new()
            {
                TemplatePath = new(""),
                NameConfigurationBuilder = new(operationConfiguration?.ListItemDtoName ??
                                               "{{entity_name_plural}}ListItemDto")
            },
            Filter = new()
            {
                TemplatePath = new("{{templates_base_path}}.GetList.GetListFilter.txt"),
                NameConfigurationBuilder = new(operationConfiguration?.FilterName ??
                                               "{{operation_name}}{{entity_name_plural}}Filter")
            },
            Handler = new()
            {
                TemplatePath = new(""),
                NameConfigurationBuilder = new(operationConfiguration?.HandlerName ??
                                               "{{operation_name}}{{entity_name_plural}}Handler")
            },
            Endpoint = new()
            {
                // If general generate is false, than endpoint generate is also false
                Generate = operationConfiguration?.Generate != false &&
                           (operationConfiguration?.GenerateEndpoint ?? true),
                TemplatePath = new("{{templates_base_path}}.GetList.GetListEndpoint.txt"),
                NameConfigurationBuilder = new(operationConfiguration?.EndpointClassName ??
                                               "{{operation_name}}{{entity_name_plural}}Endpoint"),
                FunctionName = new(operationConfiguration?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurationBuilder = new(operationConfiguration?.RouteName ?? "/{{entity_name}}")
            }
        };
    }
}