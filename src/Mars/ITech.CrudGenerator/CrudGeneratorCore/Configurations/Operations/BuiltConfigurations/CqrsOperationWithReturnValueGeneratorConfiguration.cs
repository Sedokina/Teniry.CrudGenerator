using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations.TypedBuiltConfigurations;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;

internal class CqrsOperationWithReturnValueGeneratorConfiguration
    : CqrsOperationWithoutReturnValueGeneratorConfiguration
{
    public FileTemplateBasedOperationConfiguration Dto { get; set; } = null!;
}