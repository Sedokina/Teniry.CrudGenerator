using Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations.TypedBuiltConfigurations;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;

internal class CqrsListOperationGeneratorConfiguration : CqrsOperationWithReturnValueGeneratorConfiguration
{
    public FileTemplateBasedOperationConfiguration Filter { get; set; }
    public FileTemplateBasedOperationConfiguration DtoListItem { get; set; }
}