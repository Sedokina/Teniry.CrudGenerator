using Mars.Generators.CrudGeneratorCore.Configurations.Global;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;

namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

internal class GetListQueryDefaultConfigurationBulderFactory
{
    public static CqrsListOperationConfigurationBuilder Construct(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration)
    {
        return new CqrsListOperationConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            OperationType = CqrsOperationType.Query,
            OperationGroup = new NameConfigurationBuilder("GetList{{entity_name}}"),
            Operation = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.GetList.GetListQuery.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("Get{{entity_name_plural}}Query")
            },
            Dto = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.GetList.GetListDto.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("{{entity_name_plural}}Dto")
            },
            DtoListItem = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.GetList.GetListItemDto.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("{{entity_name_plural}}ListItemDto")
            },
            Filter = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.GetList.GetListFilter.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("Get{{entity_name_plural}}Filter")
            },
            Handler = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.GetList.GetListHandler.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("Get{{entity_name_plural}}Handler")
            },
            Endpoint = new MinimalApiEndpointConfigurationBuilder
            {
                TemplatePath = new("{{templates_base_path}}.GetList.GetListEndpoint.txt"),
                NameConfigurationBuilder = new NameConfigurationBuilder("Get{{entity_name_plural}}Endpoint"),
                FunctionName = new("GetAsync"),
                RouteConfigurationBuilder = new EndpointRouteConfigurationBuilder("/{{entity_name}}")
            }
        };
    }
}