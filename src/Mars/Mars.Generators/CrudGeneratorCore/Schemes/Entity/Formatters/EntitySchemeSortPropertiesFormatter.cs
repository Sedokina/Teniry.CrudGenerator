using System.Collections.Generic;
using System.Linq;
using Mars.Generators.CrudGeneratorCore.Schemes.Entity.Properties;

namespace Mars.Generators.CrudGeneratorCore.Schemes.Entity.Formatters;

internal static class EntitySchemeSortPropertiesFormatter {
    public static string FormatAsSortKeys(this List<EntityProperty> properties)
    {
        var sortKeys = properties.Select(x => $"\"{x.SortKey}\"");
        return string.Join(",", sortKeys);
    }

    public static string FormatAsSortCalls(this List<EntityProperty> properties)
    {
        var result = properties
            .Select(property => $"{{ \"{property.SortKey}\", x => x.{property.PropertyName} }}")
            .ToList();
        return string.Join(",", result);
    }
}