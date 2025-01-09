using ITech.CrudGenerator.CrudGeneratorCore.Configurations.GeneratorConfigurations.TypedConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.GeneratorConfigurations;

internal class CqrsListOperationGeneratorConfiguration : CqrsOperationWithReturnValueGeneratorConfiguration
{
    public string Filter { get; set; } = null!;
    public string DtoListItem { get; set; } = null!;

    public CqrsListOperationGeneratorConfiguration(bool generate,
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        CqrsOperationType operationType,
        string operationName,
        NameConfigurationBuilder operationGroup,
        NameConfigurationBuilder operation,
        NameConfigurationBuilder dto,
        NameConfigurationBuilder filter,
        NameConfigurationBuilder dtoListItem,
        NameConfigurationBuilder handler,
        MinimalApiEndpointConfigurationBuilder endpoint,
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