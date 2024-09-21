using System.Collections.Generic;
using System.Linq;
using Scriban;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.TypedConfigurations;

/// <summary>
///     Available string in name:
///     - {{entity_name}}<br />
///     - {{id_param_name}}<br />
/// </summary>
public class EndpointRouteConfiguration(string name, string functionName)
{
    public string FunctionName { get; } = functionName;

    public string GetRoute(string entityName, List<string>? idParams = null)
    {
        var putIntoNamespaceTemplate = Template.Parse(name);
        entityName = entityName.ToLower();

        if (idParams == null) return putIntoNamespaceTemplate.Render(new { entityName });

        var idParamName = string.Join("/", idParams.Select(x => $"{{{x}}}"));
        return putIntoNamespaceTemplate.Render(new { entityName, idParamName });
    }
}