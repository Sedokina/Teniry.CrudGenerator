using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using Scriban;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;

/// <summary>
///     Available string keys in namespace path:<br />
///     - {{entity_assembly_name}} <br />
///     - {{business_logic_feature_name}}<br />
///     - {{operation_group}}<br />
///     - {{entity_name}}<br />
///     - {{entity_name_plural}}<br />
/// </summary>
internal class PutBusinessLogicIntoNamespaceConfigurationBuilder(string namespacePath)
{
    public string GetNamespacePath(
        string entityAssemblyName,
        string businessLogicFeatureName,
        string operationGroup,
        EntityName entityName)
    {
        var putIntoNamespaceTemplate = Template.Parse(namespacePath);
        return putIntoNamespaceTemplate.Render(new
        {
            EntityAssemblyName = entityAssemblyName,
            BusinessLogicFeatureName = businessLogicFeatureName,
            OperationGroup = operationGroup,
            EntityName = entityName.Name,
            EntityNamePlural = entityName.PluralName,
        });
    }
}