using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Scriban;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;

/// <summary>
///     Available string keys in namespace path:<br />
///     - {{assembly_name}} <br />
///     - {{feature_name}}<br />
///     - {{function_name}}<br />
/// </summary>
public class PutBusinessLogicIntoNamespaceConfigurationBuilder(string namespacePath)
{
    public string GetNamespacePath(
        string businessLogicAssemblyName,
        string businessLogicFeatureName,
        string operationName,
        EntityName entityName)
    {
        var putIntoNamespaceTemplate = Template.Parse(namespacePath);
        return putIntoNamespaceTemplate.Render(new
        {
            BusinessLogicAssemblyName = businessLogicAssemblyName,
            BusinessLogicFeatureName = businessLogicFeatureName,
            OperationName = operationName,
            EntityName = entityName.Name,
            EntityNamePlural = entityName.PluralName,
        });
    }
}