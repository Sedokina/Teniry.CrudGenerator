using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.BuildersFactories;

internal class GetListQueryDefaultConfigurationBulderFactory
{
    public static CqrsListOperationConfigurationBuilder Construct(
        GlobalCqrsGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration)
    {
        return new CqrsListOperationConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            OperationType = CqrsOperationType.Query,
            OperationName = new NameConfigurationBuilder("GetList{{entity_name}}"),
            Operation = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetList.GetListQuery.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Get{{entity_name_plural}}Query")
            },
            Dto = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetList.GetListDto.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("{{entity_name_plural}}Dto")
            },
            DtoListItem = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetList.GetListItemDto.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("{{entity_name_plural}}ListItemDto")
            },
            Filter = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetList.GetListFilter.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Get{{entity_name_plural}}Filter")
            },
            Handler = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetList.GetListHandler.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Get{{entity_name_plural}}Handler")
            },
            Endpoint = new MinimalApiEndpointConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.GetList.GetListEndpoint.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Get{{entity_name_plural}}Endpoint"),
                FunctionName = new("GetAsync"),
                RouteConfigurationBuilder = new EndpointRouteConfigurationBuilder("/{{entity_name}}")
            }
        };
    }
}