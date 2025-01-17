using ITech.CrudGenerator.Core.Schemes.Entity;
using Scriban;

namespace ITech.CrudGenerator.Core.Configurations.Configurators;

/// <summary>
///     Available string in name:
///     - {{entity_name}}<br />
///     - {{entity_name_plural}}<br />
/// </summary>
internal class NameConfigurator(string name) {
    public string GetName(EntityName entityName, string operationName) {
        var template = Template.Parse(name);
        var model = new {
            EntityName = entityName.Name,
            EntityNamePlural = entityName.PluralName,
            OperationName = operationName
        };

        return template.Render(model);
    }
}