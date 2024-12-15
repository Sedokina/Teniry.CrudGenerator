using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.EntityCustomization;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

internal class UpdateCommandDefaultConfigurationBuilderFactory
{
    public CqrsOperationWithoutReturnValueWithReceiveViewModelConfigurationBuilder Construct(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        EntityUpdateOperationCustomizationScheme? customizationScheme)
    {
        return new CqrsOperationWithoutReturnValueWithReceiveViewModelConfigurationBuilder
        {
            GlobalConfiguration = globalConfiguration,
            OperationsSharedConfiguration = operationsSharedConfiguration,
            Generate = customizationScheme?.Generate ?? true,
            OperationType = CqrsOperationType.Command,
            OperationName = customizationScheme?.Operation ?? "Update",
            OperationGroup = new(customizationScheme?.OperationGroup ?? "{{operation_name}}{{entity_name}}"),
            Operation = new()
            {
                TemplatePath = new("{{templates_base_path}}.Update.UpdateCommand.txt"),
                NameConfigurationBuilder = new(customizationScheme?.CommandName ??
                                               "{{operation_name}}{{entity_name}}Command")
            },
            Handler = new()
            {
                TemplatePath = new("{{templates_base_path}}.Update.UpdateHandler.txt"),
                NameConfigurationBuilder = new(customizationScheme?.HandlerName ??
                                               "{{operation_name}}{{entity_name}}Handler")
            },
            ViewModel = new()
            {
                TemplatePath = new("{{templates_base_path}}.Update.UpdateVm.txt"),
                NameConfigurationBuilder = new(customizationScheme?.ViewModelName ??
                                               "{{operation_name}}{{entity_name}}Vm")
            },
            Endpoint = new()
            {
                // If general generate is false, than endpoint generate is also false
                Generate = customizationScheme?.Generate != false && (customizationScheme?.GenerateEndpoint ?? true),
                TemplatePath = new("{{templates_base_path}}.Update.UpdateEndpoint.txt"),
                NameConfigurationBuilder = new(customizationScheme?.EndpointClassName ??
                                               "{{operation_name}}{{entity_name}}Endpoint"),
                FunctionName = new(customizationScheme?.EndpointFunctionName ?? "{{operation_name}}Async"),
                RouteConfigurationBuilder = new(customizationScheme?.RouteName ??
                                                "/{{entity_name}}/{{id_param_name}}/{{operation_name | string.downcase}}")
            }
        };
    }
}