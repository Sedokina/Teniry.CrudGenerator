using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.Properties;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.Formatters;

internal static class EntitySchemePropertiesFormatter
{
    public static string FormatAsProperties(this List<EntityProperty> properties)
    {
        var stringBuilder = new StringBuilder();
        foreach (var property in properties)
        {
            stringBuilder.AppendLine($"public {property.TypeName} {property.PropertyName} {{ get; set; }}");
            if (property.DefaultValue is not null)
            {
                stringBuilder.Append($" = {property.DefaultValue};");
            }
        }

        return stringBuilder.ToString().Trim();
    }

    public static string FormatAsMethodDeclarationParameters(this List<EntityProperty> properties)
    {
        var result = properties.Select(x => $"{x.TypeName} {x.PropertyNameAsMethodParameterName}");
        return string.Join("", result);
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
        return string.Join("", result);
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

    public static List<string> GetAsMethodCallArguments(this List<EntityProperty> properties)
    {
        var result = properties.Select(x => x.PropertyNameAsMethodParameterName).ToList();
        return result;
    }

    public static string FormatAsMethodCallParameters(this List<EntityProperty> properties, string objectPrefix = "")
    {
        var result = GetAsMethodCallParameters(properties, objectPrefix);
        return string.Join(", ", result);
    }

    public static string FormatAsMethodCallArguments(this List<EntityProperty> properties)
    {
        var result = properties.Select(x => x.PropertyNameAsMethodParameterName);
        return string.Join(", ", result);
    }
}