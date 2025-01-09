using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;

internal class CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration
    : CqrsOperationWithoutReturnValueGeneratorConfiguration
{
    public string ViewModel { get; set; } = null!;

    public CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration(bool generate,
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        CqrsOperationType operationType,
        string operationName,
        NameConfigurationBuilder operationGroup,
        NameConfigurationBuilder operation,
        NameConfigurationBuilder handler,
        NameConfigurationBuilder viewModel,
        MinimalApiEndpointConfigurationBuilder endpoint,
        EntityScheme entityScheme)
        : base(generate,
            globalConfiguration,
            operationsSharedConfiguration,
            operationType,
            operationName,
            operationGroup,
            operation,
            handler,
            endpoint,
            entityScheme)
    {
        ViewModel = viewModel.GetName(entityScheme.EntityName, OperationName);
    }
}