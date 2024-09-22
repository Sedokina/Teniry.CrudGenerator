using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Scriban;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;

/// <summary>
///     Available string keys in namespace path:<br />
///     - {{entity_assembly_name}} <br />
///     - {{business_logic_feature_name}}<br />
///     - {{operation_name}}<br />
///     - {{entity_name}}<br />
///     - {{entity_name_plural}}<br />
/// </summary>
public class PutBusinessLogicIntoNamespaceConfigurationBuilder(string namespacePath)
{
    public string GetNamespacePath(
        string entityAssemblyName,
        string businessLogicFeatureName,
        string operationName,
        EntityName entityName)
    {
        var putIntoNamespaceTemplate = Template.Parse(namespacePath);
        return putIntoNamespaceTemplate.Render(new
        {
            EntityAssemblyName = entityAssemblyName,
            BusinessLogicFeatureName = businessLogicFeatureName,
            OperationName = operationName,
            EntityName = entityName.Name,
            EntityNamePlural = entityName.PluralName,
        });
    }
}