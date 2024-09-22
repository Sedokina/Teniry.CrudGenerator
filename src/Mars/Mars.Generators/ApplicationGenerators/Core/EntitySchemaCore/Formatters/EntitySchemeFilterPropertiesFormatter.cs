using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Properties;

namespace Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Formatters;

internal static class EntitySchemeFilterPropertiesFormatter {
    public static string FormatAsFilterProperties(this List<EntityProperty> properties)
    {
        var stringBuilder = new StringBuilder();
        foreach (var filterProperty in properties.SelectMany(property => property.FilterProperties))
        {
            stringBuilder
                .AppendLine($"public {filterProperty.TypeName} {filterProperty.PropertyName} {{ get; set; }}");
        }

        return stringBuilder.ToString();
    }

    public static string FormatAsFilterBody(this List<EntityProperty> properties)
    {
        var stringBuilder = new StringBuilder();

        foreach (var property in properties)
        {
            foreach (var filterProperty in property.FilterProperties)
            {
                filterProperty.FilterExpression.Format(
                    stringBuilder,
                    filterProperty.PropertyName,
                    property.PropertyName);
            }
        }


        return stringBuilder.ToString();
    }
}