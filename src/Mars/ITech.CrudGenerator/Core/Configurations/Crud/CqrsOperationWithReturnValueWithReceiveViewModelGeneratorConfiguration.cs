using ITech.CrudGenerator.Core.Configurations.Configurators;
using ITech.CrudGenerator.Core.Configurations.Crud.TypedConfigurations;
using ITech.CrudGenerator.Core.Configurations.Global;
using ITech.CrudGenerator.Core.Configurations.Shared;
using ITech.CrudGenerator.Core.Schemes.Entity;

namespace ITech.CrudGenerator.Core.Configurations.Crud;

internal record CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration
    : CqrsOperationWithoutReturnValueGeneratorConfiguration {
    public string ViewModel { get; set; } = null!;

    public CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration(
        bool generate,
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        CqrsOperationType operationType,
        string operationName,
        NameConfigurator operationGroup,
        NameConfigurator operation,
        NameConfigurator handler,
        NameConfigurator viewModel,
        MinimalApiEndpointConfigurator endpoint,
        EntityScheme entityScheme
    )
        : base(
            generate,
            globalConfiguration,
            operationsSharedConfiguration,
            operationType,
            operationName,
            operationGroup,
            operation,
            handler,
            endpoint,
            entityScheme
        ) {
        ViewModel = viewModel.GetName(entityScheme.EntityName, OperationName);
    }
}