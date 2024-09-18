using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Properties;

namespace Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Formatters;

public static class EntitySchemePropertiesFormatter
{
    public static string FormatAsProperties(this List<EntityProperty> properties)
    {
        var stringBuilder = new StringBuilder();
        foreach (var property in properties)
        {
            stringBuilder.AppendLine($"\tpublic {property.TypeName} {property.PropertyName} {{ get; set; }}");
        }

        return stringBuilder.ToString().Trim();
    }

    public static string FormatAsMethodDeclarationParameters(this List<EntityProperty> properties)
    {
        var result = properties.Select(x => $"{x.TypeName} {x.PropertyNameAsMethodParameterName}");
        return string.Join("\n\t\t", result);
    }

    public static string FormatAsConstructorBody(this List<EntityProperty> properties)
    {
        var result = properties.Select(x =>
        {
            if (x.PropertyName.Equals(x.PropertyNameAsMethodParameterName))
            {
                return $"this.{x.PropertyName} = {x.PropertyNameAsMethodParameterName};";
            }

            return $"{x.PropertyName} = {x.PropertyNameAsMethodParameterName};";
        });
        return string.Join("\n\t\t", result);
    }

    public static List<string> GetAsMethodCallParameters(this List<EntityProperty> properties, string objectPrefix = "")
    {
        objectPrefix = !string.IsNullOrEmpty(objectPrefix) && !objectPrefix.EndsWith(".")
            ? objectPrefix + "."
            : objectPrefix;
        return properties
            .Select(x => $"{objectPrefix}{x.PropertyName}")
            .ToList();
    }

    public static string FormatAsMethodCallParameters(this List<EntityProperty> properties, string objectPrefix = "")
    {
        var result = GetAsMethodCallParameters(properties, objectPrefix);
        return string.Join(", ", result);
    }
}