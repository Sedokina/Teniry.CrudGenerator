using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Global.TypedConfigurations;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.TypedConfigurations;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Factories;

public class DeleteCommandDefaultConfigurationFactory
{
    public static CqrsOperationGeneratorConfiguration Construct(GlobalCqrsGeneratorConfiguration globalConfiguration)
    {
        return new CqrsOperationGeneratorConfiguration
        {
            GlobalConfiguration = globalConfiguration,
            OperationType = CqrsOperationType.Command,
            FunctionName = new NameConfiguration("Delete{{entity_name}}"),
            Operation = new FileTemplateBasedOperationConfiguration
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Delete.DeleteCommand.txt",
                NameConfiguration = new NameConfiguration("Delete{{entity_name}}Command")
            },
            Handler = new FileTemplateBasedOperationConfiguration
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Delete.DeleteHandler.txt",
                NameConfiguration = new NameConfiguration("Delete{{entity_name}}Handler")
            },
            Endpoint = new MinimalApiEndpointConfiguration
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Delete.DeleteEndpoint.txt",
                NameConfiguration = new NameConfiguration("Delete{{entity_name}}Endpoint"),
                RouteConfiguration =
                    new EndpointRouteConfiguration("/{{entity_name}}/{{id_param_name}}/delete", "DeleteAsync")
            }
        };
    }
}