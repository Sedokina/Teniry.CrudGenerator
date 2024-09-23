using Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations.TypedBuiltConfigurations;

namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;

internal class CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration
    : CqrsOperationWithoutReturnValueGeneratorConfiguration
{
    public FileTemplateBasedOperationConfiguration ViewModel { get; set; } = null!;
}