using Mars.Generators.CrudGeneratorCore.Configurations.Global;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;

namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

internal class UpdateCommandDefaultConfigurationBuilderFactory
{
    public static CqrsOperationWithoutReturnValueConfigurationBuilder Construct(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration)
    {
        return new CqrsOperationWithoutReturnValueConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            OperationType = CqrsOperationType.Command,
            OperationName = new NameConfigurationBuilder("Update{{entity_name}}"),
            Operation = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Update.UpdateCommand.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Update{{entity_name}}Command")
            },
            Handler = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Update.UpdateHandler.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Update{{entity_name}}Handler")
            },
            Endpoint = new MinimalApiEndpointConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Update.UpdateEndpoint.txt",
                NameConfigurationBuilder = new NameConfigurationBuilder("Update{{entity_name}}Endpoint"),
                FunctionName = new("UpdateAsync"),
                RouteConfigurationBuilder =
                    new EndpointRouteConfigurationBuilder("/{{entity_name}}/{{id_param_name}}/update")
            }
        };
    }
}