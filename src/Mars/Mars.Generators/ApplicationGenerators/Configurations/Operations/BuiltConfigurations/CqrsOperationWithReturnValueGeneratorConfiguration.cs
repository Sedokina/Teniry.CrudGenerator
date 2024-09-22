using Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations.TypedBuiltConfigurations;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;

public class CqrsOperationWithReturnValueGeneratorConfiguration : CqrsOperationWithoutReturnValueGeneratorConfiguration
{
    public FileTemplateBasedOperationConfiguration Dto { get; set; }
}