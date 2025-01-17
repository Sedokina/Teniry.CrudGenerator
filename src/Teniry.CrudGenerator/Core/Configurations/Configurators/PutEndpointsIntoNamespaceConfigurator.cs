using Scriban;
using Teniry.CrudGenerator.Core.Schemes.Entity;

namespace Teniry.CrudGenerator.Core.Configurations.Configurators;

/// <summary>
///     Available string keys in namespace path:<br />
///     - {{entity_assembly_name}}<br />
///     - {{entity_name}}<br />
///     - {{entity_name_plural}}<br />
/// </summary>
internal class PutEndpointsIntoNamespaceConfigurator(string namespacePath) {
    public string GetNamespacePath(
        EntityName entityName,
        string entityAssemblyName
    ) {
        var putIntoNamespaceTemplate = Template.Parse(namespacePath);

        return putIntoNamespaceTemplate.Render(
            new {
                EntityName = entityName.Name,
                EntityNamePlural = entityName.PluralName,
                EntityAssemblyName = entityAssemblyName
            }
        );
    }
}