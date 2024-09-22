using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Scriban;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;

/// <summary>
///     Available string keys in namespace path:<br />
///     - {{entity_assembly_name}}<br />
///     - {{entity_name}}<br />
///     - {{entity_name_plural}}<br />
/// </summary>
public class PutEndpointsIntoNamespaceConfigurationBuilder(string namespacePath)
{
    public string GetNamespacePath(
        EntityName entityName,
        string entityAssemblyName)
    {
        var putIntoNamespaceTemplate = Template.Parse(namespacePath);
        return putIntoNamespaceTemplate.Render(new
        {
            EntityName = entityName.Name,
            EntityNamePlural = entityName.PluralName,
            EntityAssemblyName = entityAssemblyName,
        });
    }
}