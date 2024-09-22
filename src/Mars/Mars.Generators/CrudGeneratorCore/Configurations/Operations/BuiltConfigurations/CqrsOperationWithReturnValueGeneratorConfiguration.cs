using Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations.TypedBuiltConfigurations;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;

internal class CqrsOperationWithReturnValueGeneratorConfiguration
    : CqrsOperationWithoutReturnValueGeneratorConfiguration
{
    public FileTemplateBasedOperationConfiguration Dto { get; set; } = null!;
}