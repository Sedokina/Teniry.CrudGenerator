using System.Collections.Generic;
using Mars.Generators.Extensions;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Core;

public static class PropertiesExtractor
{
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
    public static List<string> GetPrimaryKeyNamesOfEntity(ISymbol symbol, string objectPrefix)
    {
        objectPrefix = !string.IsNullOrEmpty(objectPrefix) && !objectPrefix.EndsWith(".")
            ? objectPrefix + "."
            : objectPrefix;
        var result = new List<string>();

        var propertiesOfClass = ((INamedTypeSymbol)symbol).GetMembers().OfType<IPropertySymbol>();
        foreach (var propertySymbol in propertiesOfClass)
        {
            // skip adding to command property if it is not id of the entity
            var propertyNameLower = propertySymbol.Name.ToLower();
            if (!propertyNameLower.Equals("id") && !propertyNameLower.Equals($"{symbol.Name}id")) continue;

            result.Add($"{objectPrefix}{propertySymbol.Name}");
        }

        return result;
    }

    public static string GetPrimaryKeysOfEntityAsProperties(ISymbol symbol)
    {
        var propertiesOfClass = ((INamedTypeSymbol)symbol).GetMembers().OfType<IPropertySymbol>();
        var result = "";
        foreach (var propertySymbol in propertiesOfClass)
        {
            // skip adding to query property if it is not id of the entity
            var propertyNameLower = propertySymbol.Name.ToLower();
            if (!propertyNameLower.Equals("id") && !propertyNameLower.Equals($"{symbol.Name}id")) continue;

            // For DateTimeOffset and other date variations remove system from the property type declaration
            var propertyTypeName = propertySymbol.Type.ToString().ToLower().StartsWith("system.")
                ? propertySymbol.Type.MetadataName
                : propertySymbol.Type.ToString();

            result += $"public {propertyTypeName} {propertySymbol.Name} {{ get; set; }}\n\t";
        }

        result = result.TrimEnd();
        return result;
    }

    public static string GetAllPropertiesOfEntity(ISymbol symbol, bool skipPrimaryKeys = false)
    {
        var propertiesOfClass = ((INamedTypeSymbol)symbol).GetMembers().OfType<IPropertySymbol>();
        var result = "";
        foreach (var propertySymbol in propertiesOfClass)
        {
            var propertyNameLower = propertySymbol.Name.ToLower();
            if (skipPrimaryKeys &&
                (propertyNameLower.Equals("id") || propertyNameLower.Equals($"{symbol.Name}id"))) continue;

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