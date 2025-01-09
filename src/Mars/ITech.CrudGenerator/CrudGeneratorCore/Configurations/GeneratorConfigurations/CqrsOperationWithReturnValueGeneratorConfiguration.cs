using ITech.CrudGenerator.CrudGeneratorCore.Configurations.GeneratorConfigurations.TypedConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.GeneratorConfigurations;

internal class CqrsOperationWithReturnValueGeneratorConfiguration
    : CqrsOperationWithoutReturnValueGeneratorConfiguration
{
    public string Dto { get; set; } = null!;

    public CqrsOperationWithReturnValueGeneratorConfiguration(bool generate,
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        CqrsOperationType operationType,
        string operationName,
        NameConfigurationBuilder operationGroup,
        NameConfigurationBuilder operation,
        NameConfigurationBuilder dto,
        NameConfigurationBuilder handler,
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
        Dto = dto.GetName(entityScheme.EntityName, OperationName);
    }
}