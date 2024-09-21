using Mars.Generators.ApplicationGenerators.Configurations.Operations.TypedConfigurations;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations;

public class CqrsOperationWithReturnValueGeneratorConfiguration : CqrsOperationGeneratorConfiguration
{
    public FileTemplateBasedOperationConfiguration Dto { get; set; }
}