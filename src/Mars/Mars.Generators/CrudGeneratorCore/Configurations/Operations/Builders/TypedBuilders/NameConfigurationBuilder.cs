using Mars.Generators.CrudGeneratorCore.Schemes.Entity;
using Scriban;

namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;

/// <summary>
///     Available string in name:
///     - {{entity_name}}<br />
///     - {{entity_name_plural}}<br />
/// </summary>
internal class NameConfigurationBuilder(string name)
{
    public string GetName(EntityName entityName)
    {
        var template = Template.Parse(name);
        var model = new
        {
            EntityName = entityName.Name,
            EntityNamePlural = entityName.PluralName
        };
        return template.Render(model);
    }
}