using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Scriban;

namespace Mars.Generators.ApplicationGenerators.Configurations.Global.TypedConfigurations;

/// <summary>
///     Available string keys in namespace path:<br />
///     - {{assembly_name}} <br />
///     - {{feature_name}}<br />
///     - {{function_name}}<br />
/// </summary>
public class PutBusinessLogicIntoNamespaceConfiguration(string namespacePath)
{
    public string GetNamespacePath(
        EntityName entityName,
        string assemblyName,
        NameConfiguration featureName,
        NameConfiguration functionNameConfiguration)
    {
        var putIntoNamespaceTemplate = Template.Parse(namespacePath);
        return putIntoNamespaceTemplate.Render(new
        {
            AssemblyName = assemblyName,
            FeatureName = featureName.GetName(entityName),
            FunctionName = functionNameConfiguration.GetName(entityName)
        });
    }
}