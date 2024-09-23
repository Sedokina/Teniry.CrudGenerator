using System.Collections.Generic;
using System.Linq;
using Scriban;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;

/// <summary>
///     Available string in name:
///     - {{entity_name}}<br />
///     - {{id_param_name}}<br />
/// </summary>
internal class EndpointRouteConfigurationBuilder(string name)
{
    public string GetRoute(string entityName, string operationName, List<string>? idParams = null)
    {
        var template = Template.Parse(name);
        entityName = entityName.ToLower();

        if (idParams == null) return template.Render(new { entityName });

        var idParamName = string.Join("/", idParams.Select(x => $"{{{x}}}"));
        return template.Render(new { entityName, idParamName, operationName });
    }
}