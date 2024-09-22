using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;

public class FileTemplateBasedOperationConfigurationBuilder
{
    public string TemplatePath { get; set; } = null!;
    public NameConfigurationBuilder NameConfigurationBuilder { get; set; } = null!;

    // TODO: remove direct usage of entity name, get it from entity
    public string GetName(EntityName entityName)
    {
        return NameConfigurationBuilder.GetName(entityName);
    }
}