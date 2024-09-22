using Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations.TypedBuiltConfigurations;

namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;

internal class CqrsOperationWithReturnValueGeneratorConfiguration
    : CqrsOperationWithoutReturnValueGeneratorConfiguration
{
    public FileTemplateBasedOperationConfiguration Dto { get; set; } = null!;
}