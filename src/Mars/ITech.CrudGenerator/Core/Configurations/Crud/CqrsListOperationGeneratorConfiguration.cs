using ITech.CrudGenerator.Core.Configurations.Configurators;
using ITech.CrudGenerator.Core.Configurations.Crud.TypedConfigurations;
using ITech.CrudGenerator.Core.Configurations.Global;
using ITech.CrudGenerator.Core.Configurations.Shared;
using ITech.CrudGenerator.Core.Schemes.Entity;

namespace ITech.CrudGenerator.Core.Configurations.Crud;

internal record CqrsListOperationGeneratorConfiguration : CqrsOperationWithReturnValueGeneratorConfiguration
{
    public string Filter { get; set; } = null!;
    public string DtoListItem { get; set; } = null!;

    public CqrsListOperationGeneratorConfiguration(bool generate,
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
        : base(generate,
            globalConfiguration,
            operationsSharedConfiguration,
            operationType,
            operationName,
            operationGroup,
            operation,
            dto,
            handler,
            endpoint,
            entityScheme)
    {
        Filter = filter.GetName(entityScheme.EntityName, OperationName);
        DtoListItem = dtoListItem.GetName(entityScheme.EntityName, OperationName);
    }
}