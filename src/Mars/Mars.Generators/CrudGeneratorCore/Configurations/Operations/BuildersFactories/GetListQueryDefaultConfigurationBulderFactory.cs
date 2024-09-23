using Mars.Generators.CrudGeneratorCore.Configurations.Global;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders;
using Mars.Generators.CrudGeneratorCore.Schemes.EntityCustomization;

namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

internal class GetListQueryDefaultConfigurationBulderFactory
{
    public static CqrsListOperationConfigurationBuilder Construct(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        EntityGetListOperationCustomizationScheme? customizationScheme)
    {
        return new CqrsListOperationConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            Generate = customizationScheme?.Generate ?? true,
            OperationType = CqrsOperationType.Query,
            OperationName = customizationScheme?.OperationType ?? "Get",
            OperationGroup = new(customizationScheme?.OperationGroup ?? "{{operation_name}}{{entity_name_plural}}"),
            Operation = new()
            {
                TemplatePath = new("{{templates_base_path}}.GetList.GetListQuery.txt"),
                NameConfigurationBuilder = new(customizationScheme?.OperationName ??
                                               "{{operation_name}}{{entity_name_plural}}Query")
            },
            Dto = new()
            {
                TemplatePath = new("{{templates_base_path}}.GetList.GetListDto.txt"),
                NameConfigurationBuilder = new(customizationScheme?.DtoName ??
                                               "{{entity_name_plural}}Dto")
            },
            DtoListItem = new()
            {
                TemplatePath = new("{{templates_base_path}}.GetList.GetListItemDto.txt"),
                NameConfigurationBuilder = new(customizationScheme?.ListItemDtoName ??
                                               "{{entity_name_plural}}ListItemDto")
            },
            Filter = new()
            {
                TemplatePath = new("{{templates_base_path}}.GetList.GetListFilter.txt"),
                NameConfigurationBuilder = new(customizationScheme?.FilterName ??
                                               "{{operation_name}}{{entity_name_plural}}Filter")
            },
            Handler = new()
            {
                TemplatePath = new("{{templates_base_path}}.GetList.GetListHandler.txt"),
                NameConfigurationBuilder = new(customizationScheme?.HandlerName ??
                                               "{{operation_name}}{{entity_name_plural}}Handler")
            },
            Endpoint = new()
            {
                // If general generate is false, than endpoint generate is also false
                Generate = customizationScheme?.Generate != false && (customizationScheme?.GenerateEndpoint ?? true),
                TemplatePath = new("{{templates_base_path}}.GetList.GetListEndpoint.txt"),
                NameConfigurationBuilder = new(customizationScheme?.EndpointClassName ??
                                               "{{operation_name}}{{entity_name_plural}}Endpoint"),
                FunctionName = new(customizationScheme?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurationBuilder = new(customizationScheme?.RouteName ?? "/{{entity_name}}")
            }
        };
    }
}