using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Configurators;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Crud.TypedConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Shared;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Crud;

internal class CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration
    : CqrsOperationWithoutReturnValueGeneratorConfiguration
{
    public string ViewModel { get; set; } = null!;

    public CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration(bool generate,
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        CqrsOperationType operationType,
        string operationName,
        NameConfigurator operationGroup,
        NameConfigurator operation,
        NameConfigurator handler,
        NameConfigurator viewModel,
        MinimalApiEndpointConfigurator endpoint,
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