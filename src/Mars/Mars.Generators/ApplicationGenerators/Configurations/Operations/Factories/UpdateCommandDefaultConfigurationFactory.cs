using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Global.TypedConfigurations;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.TypedConfigurations;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Factories;

public class UpdateCommandDefaultConfigurationFactory
{
    public static CqrsOperationGeneratorConfiguration Construct(GlobalCqrsGeneratorConfiguration globalConfiguration)
    {
        return new CqrsOperationGeneratorConfiguration
        {
            GlobalConfiguration = globalConfiguration,
            OperationType = CqrsOperationType.Command,
            FunctionName = new NameConfiguration("Update{{entity_name}}"),
            Operation = new FileTemplateBasedOperationConfiguration
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Update.UpdateCommand.txt",
                NameConfiguration = new NameConfiguration("Update{{entity_name}}Command")
            },
            Handler = new FileTemplateBasedOperationConfiguration
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Update.UpdateHandler.txt",
                NameConfiguration = new NameConfiguration("Update{{entity_name}}Handler")
            },
            Endpoint = new MinimalApiEndpointConfiguration
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Update.UpdateEndpoint.txt",
                NameConfiguration = new NameConfiguration("Update{{entity_name}}Endpoint"),
                RouteConfiguration =
                    new EndpointRouteConfiguration("/{{entity_name}}/{{id_param_name}}/update", "UpdateAsync")
            }
        };
    }
}