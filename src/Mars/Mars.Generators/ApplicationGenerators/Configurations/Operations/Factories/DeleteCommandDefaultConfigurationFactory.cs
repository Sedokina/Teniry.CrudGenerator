using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.TypedConfigurations;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Factories;

public class DeleteCommandDefaultConfigurationFactory
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
            FunctionName = new NameConfiguration("Delete{{entity_name}}"),
            Operation = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Delete.DeleteCommand.txt",
                NameConfiguration = new NameConfiguration("Delete{{entity_name}}Command")
            },
            Handler = new FileTemplateBasedOperationConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Delete.DeleteHandler.txt",
                NameConfiguration = new NameConfiguration("Delete{{entity_name}}Handler")
            },
            Endpoint = new MinimalApiEndpointConfigurationBuilder
            {
                TemplatePath = $"{globalConfiguration.TemplatesBasePath}.Delete.DeleteEndpoint.txt",
                NameConfiguration = new NameConfiguration("Delete{{entity_name}}Endpoint"),
                RouteConfiguration =
                    new EndpointRouteConfiguration("/{{entity_name}}/{{id_param_name}}/delete", "DeleteAsync")
            }
        };
    }
}