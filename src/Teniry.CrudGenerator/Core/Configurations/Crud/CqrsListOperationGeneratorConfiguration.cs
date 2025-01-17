using Teniry.CrudGenerator.Core.Configurations.Configurators;
using Teniry.CrudGenerator.Core.Configurations.Crud.TypedConfigurations;
using Teniry.CrudGenerator.Core.Configurations.Global;
using Teniry.CrudGenerator.Core.Configurations.Shared;
using Teniry.CrudGenerator.Core.Schemes.Entity;

namespace Teniry.CrudGenerator.Core.Configurations.Crud;

internal record CqrsListOperationGeneratorConfiguration : CqrsOperationWithReturnValueGeneratorConfiguration {
    public string Filter { get; set; } = null!;
    public string DtoListItem { get; set; } = null!;

    public CqrsListOperationGeneratorConfiguration(
        bool generate,
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        CqrsOperationType operationType,
        string operationName,
        NameConfigurator operationGroup,
        NameConfigurator operation,
        NameConfigurator dto,
        NameConfigurator filter,
        NameConfigurator dtoListItem,
        NameConfigurator handler,
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
            dto,
            handler,
            endpoint,
            entityScheme
        ) {
        Filter = filter.GetName(entityScheme.EntityName, OperationName);
        DtoListItem = dtoListItem.GetName(entityScheme.EntityName, OperationName);
    }
}