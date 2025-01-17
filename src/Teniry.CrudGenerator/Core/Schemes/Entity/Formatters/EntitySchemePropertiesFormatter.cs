using System.Collections.Generic;
using System.Linq;
using Teniry.CrudGenerator.Core.Schemes.Entity.Properties;

namespace Teniry.CrudGenerator.Core.Schemes.Entity.Formatters;

internal static class EntitySchemePropertiesFormatter {
    public static List<string> GetAsMethodCallParameters(
        this List<EntityProperty> properties,
        string objectPrefix = ""
    ) {
        objectPrefix = !string.IsNullOrEmpty(objectPrefix) && !objectPrefix.EndsWith(".") ?
            objectPrefix + "." :
            objectPrefix;

        return properties
            .Select(x => $"{objectPrefix}{x.PropertyName}")
            .ToList();
    }

    public static List<string> GetAsMethodCallArguments(this List<EntityProperty> properties) {
        var result = properties.Select(x => x.PropertyNameAsMethodParameterName).ToList();

        return result;
    }
}