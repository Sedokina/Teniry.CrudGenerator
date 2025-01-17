using System.Collections.Generic;
using System.Linq;
using Scriban;

namespace ITech.CrudGenerator.Core.Configurations.Configurators;

/// <summary>
///     Available string in name:
///     - {{entity_name}}<br />
///     - {{id_param_name}}<br />
/// </summary>
internal class EndpointRouteConfigurator(string name)
{
    public string GetRoute(string entityName, string operationName, List<string>? idParams = null)
    {
        var template = Template.Parse(name);
        entityName = FirstCharToLowerCase(entityName);

        if (idParams == null) return template.Render(new { entityName });

        var idParamName = string.Join("/", idParams.Select(x => $"{{{x}}}"));
        return template.Render(new { entityName, idParamName, operationName });
    }

    private static string FirstCharToLowerCase(string str)
    {
        if (!string.IsNullOrEmpty(str) && char.IsUpper(str[0]))
        {
            return str.Length == 1 ? char.ToLower(str[0]).ToString() : char.ToLower(str[0]) + str.Substring(1);
        }

        return str;
    }
}