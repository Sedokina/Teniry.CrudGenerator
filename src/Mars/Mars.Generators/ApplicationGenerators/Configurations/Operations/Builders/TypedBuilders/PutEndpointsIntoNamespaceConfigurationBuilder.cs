using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Scriban;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;

/// <summary>
///     Available string keys in namespace path:<br />
///     - {{assembly_name}} <br />
///     - {{feature_name}}<br />
///     - {{function_name}}<br />
/// </summary>
public class PutEndpointsIntoNamespaceConfigurationBuilder(string namespacePath)
{
    public string GetNamespacePath(
        EntityName entityName,
        string endpointsAssemblyName)
    {
        var putIntoNamespaceTemplate = Template.Parse(namespacePath);
        return putIntoNamespaceTemplate.Render(new
        {
            EntityName = entityName.Name,
            EntityNamePlural = entityName.PluralName,
            EndpointsAssemblyName = endpointsAssemblyName,
        });
    }
}