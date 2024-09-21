using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Scriban;

namespace Mars.Generators.ApplicationGenerators.Configurations.Global.TypedConfigurations;

/// <summary>
///     Available string in name:
///     - {{entity_name}}<br />
/// </summary>
public class NameConfiguration(string name)
{
    public string GetName(EntityName entityName)
    {
        var putIntoNamespaceTemplate = Template.Parse(name);
        var model = new
        {
            EntityName = entityName.Name,
            PluralEntityName = entityName.PluralName
        };
        return putIntoNamespaceTemplate.Render(model);
    }
}