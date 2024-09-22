using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.TypedConfigurations;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Factories;

public class UpdateCommandDefaultConfigurationFactory
{
    public static CqrsOperationWithoutReturnValueGeneratorConfigurationBuilder Construct(
        GlobalCqrsGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfiguration operationsSharedConfiguration)
    {
        return new CqrsOperationWithoutReturnValueGeneratorConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            OperationType = CqrsOperationType.Command,
            FunctionName = new NameConfiguration("Update{{entity_name}}"),
            Operation = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Update.UpdateCommand.txt",
                NameConfiguration = new NameConfiguration("Update{{entity_name}}Command")
            },
            Handler = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Update.UpdateHandler.txt",
                NameConfiguration = new NameConfiguration("Update{{entity_name}}Handler")
            },
            Endpoint = new MinimalApiEndpointConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Update.UpdateEndpoint.txt",
                NameConfiguration = new NameConfiguration("Update{{entity_name}}Endpoint"),
                RouteConfiguration =
                    new EndpointRouteConfiguration("/{{entity_name}}/{{id_param_name}}/update", "UpdateAsync")
            }
        };
    }
}