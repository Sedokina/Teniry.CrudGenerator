using Teniry.CrudGenerator.Core.Configurations.Configurators;
using Teniry.CrudGenerator.Core.Configurations.Crud.TypedConfigurations;
using Teniry.CrudGenerator.Core.Configurations.Global;
using Teniry.CrudGenerator.Core.Configurations.Shared;
using Teniry.CrudGenerator.Core.Schemes.Entity;

namespace Teniry.CrudGenerator.Core.Configurations.Crud;

internal record CqrsOperationWithReturnValueGeneratorConfiguration
    : CqrsOperationWithoutReturnValueGeneratorConfiguration {
    public string Dto { get; set; } = null!;

    public CqrsOperationWithReturnValueGeneratorConfiguration(
        bool generate,
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        CqrsOperationType operationType,
        string operationName,
        NameConfigurator operationGroup,
        NameConfigurator operation,
        NameConfigurator dto,
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
            handler,
            endpoint,
            entityScheme
        ) {
        Dto = dto.GetName(entityScheme.EntityName, OperationName);
    }
}