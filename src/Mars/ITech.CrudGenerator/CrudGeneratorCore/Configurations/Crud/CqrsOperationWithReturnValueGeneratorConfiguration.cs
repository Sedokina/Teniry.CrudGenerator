using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Configurators;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Crud.TypedConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Shared;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Crud;

internal class CqrsOperationWithReturnValueGeneratorConfiguration
    : CqrsOperationWithoutReturnValueGeneratorConfiguration
{
    public string Dto { get; set; } = null!;

    public CqrsOperationWithReturnValueGeneratorConfiguration(bool generate,
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator operationsSharedConfiguration,
        CqrsOperationType operationType,
        string operationName,
        NameConfigurator operationGroup,
        NameConfigurator operation,
        NameConfigurator dto,
        NameConfigurator handler,
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
        Dto = dto.GetName(entityScheme.EntityName, OperationName);
    }
}