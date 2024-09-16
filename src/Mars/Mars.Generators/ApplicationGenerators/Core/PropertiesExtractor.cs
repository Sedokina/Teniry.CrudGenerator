using System.Collections.Generic;
using System.Linq;
using Mars.Generators.ApplicationGenerators.Core.Extensions;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Core;

public static class PropertiesExtractor
{
    public static List<PropertyNameType> GetPrimaryKeysOfEntity(ISymbol symbol)
    {
        var propertiesOfClass = ((INamedTypeSymbol)symbol).GetMembers().OfType<IPropertySymbol>();
        var result = new List<PropertyNameType>();
        foreach (var propertySymbol in propertiesOfClass)
        {
            // skip adding to query property if it is not id of the entity
            if (!isEntityId(symbol.Name, propertySymbol.Name)) continue;

            // For DateTimeOffset and other date variations remove system from the property type declaration
            var propertyTypeName = propertySymbol.Type.ToString().ToLower().StartsWith("system.")
                ? propertySymbol.Type.MetadataName
                : propertySymbol.Type.ToString();

            result.Add(new PropertyNameType(
                propertyTypeName,
                propertySymbol.Name,
                propertySymbol.Name.ToLowerFirstChar()));
        }

        return result;
    }

    /// <summary>
    ///     Get list of property names of the entity primary keys
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="objectPrefix">
    ///     If property name Id, by default it would be returned as is
    ///     if it has to be used in some expression, i.e.: myObject => myObject.Id
    ///     pass "myObject" or "myObject." so that result would be myObject.Id
    /// </param>
    /// <returns>List of names of the primary keys with or without prefix</returns>
    public static List<string> ToPropertiesNamesList(this List<PropertyNameType> properties, string objectPrefix = "")
    {
        objectPrefix = !string.IsNullOrEmpty(objectPrefix) && !objectPrefix.EndsWith(".")
            ? objectPrefix + "."
            : objectPrefix;
        return properties
            .Select(x => $"{objectPrefix}{x.PropertyName}")
            .ToList();
    }

    public static string ToClassPropertiesString(this List<PropertyNameType> properties)
    {
        var result = properties
            .Select(x => $"public {x.TypeName} {x.PropertyName} {{ get; set; }}");

        return string.Join("\n\t", result);
    }

    public static string ToMethodParametersString(this List<PropertyNameType> properties)
    {
        var result = properties.Select(x => $"{x.TypeName} {x.MethodParameterName}");
        return string.Join("\n\t\t", result);
    }

    public static string ToConstructorBodyString(this List<PropertyNameType> properties)
    {
        var result = properties.Select(x =>
        {
            if (x.PropertyName.Equals(x.MethodParameterName))
            {
                return $"this.{x.PropertyName} = {x.MethodParameterName};";
            }

            return $"{x.PropertyName} = {x.MethodParameterName};";
        });
        return string.Join("\n\t\t", result);
    }

    private static bool isEntityId(string className, string propertyName)
    {
        var lower = propertyName.ToLower();
        return lower.Equals("id") || lower.Equals($"{className}id") || lower.Equals("_id");
    }

    public static string GetAllPropertiesOfEntity(ISymbol symbol, bool skipPrimaryKeys = false)
    {
        var propertiesOfClass = ((INamedTypeSymbol)symbol).GetMembers().OfType<IPropertySymbol>();
        var result = "";
        foreach (var propertySymbol in propertiesOfClass)
        {
            if (skipPrimaryKeys && isEntityId(symbol.Name, propertySymbol.Name)) continue;

            // skip adding to command if not primitive type
            if (!propertySymbol.Type.IsSimple()) continue;

            // For DateTimeOffset and other date variations remove system from the property type declaration
            var propertyTypeName = propertySymbol.Type.ToString().ToLower().StartsWith("system.")
                ? propertySymbol.Type.MetadataName
                : propertySymbol.Type.ToString();

            result += $"public {propertyTypeName} {propertySymbol.Name} {{ get; set; }}\n\t";
        }

        result = result.TrimEnd();
        return result;
    }
}

public struct PropertyNameType
{
    public string TypeName { get; set; }
    public string PropertyName { get; set; }
    public string MethodParameterName { get; set; }

    public PropertyNameType(string typeName, string propertyName, string methodParameterName)
    {
        TypeName = typeName;
        PropertyName = propertyName;
        MethodParameterName = methodParameterName;
    }
}