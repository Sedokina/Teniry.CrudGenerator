using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Scriban;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;

/// <summary>
///     Available string in name:
///     - {{entity_name}}<br />
///     - {{entity_name_plural}}<br />
/// </summary>
public class NameConfigurationBuilder(string name)
{
    public string GetName(EntityName entityName)
    {
        var putIntoNamespaceTemplate = Template.Parse(name);
        var model = new
        {
            EntityName = entityName.Name,
            EntityNamePlural = entityName.PluralName
        };
        return putIntoNamespaceTemplate.Render(model);
    }
}