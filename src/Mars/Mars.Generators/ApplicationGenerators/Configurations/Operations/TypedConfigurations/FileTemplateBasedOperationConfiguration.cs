using Mars.Generators.ApplicationGenerators.Configurations.Global.TypedConfigurations;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.TypedConfigurations;

public class FileTemplateBasedOperationConfiguration
{
    public string TemplatePath { get; set; } = null!;
    public NameConfiguration NameConfiguration { get; set; } = null!;

    // TODO: remove direct usage of entity name, get it from entity
    public string GetName(EntityName entityName)
    {
        return NameConfiguration.GetName(entityName);
    }
}