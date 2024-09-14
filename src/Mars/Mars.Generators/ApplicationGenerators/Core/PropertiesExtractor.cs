using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Core;

public static class PropertiesExtractor
{
    public static List<string> GetPrimaryKeysOfEntity(ISymbol symbol, string fromObject)
    {
        fromObject = !string.IsNullOrEmpty(fromObject) && !fromObject.EndsWith(".") ? fromObject + "." : fromObject;
        var result = new List<string>();

        var propertiesOfClass = ((INamedTypeSymbol)symbol).GetMembers().OfType<IPropertySymbol>();
        foreach (var propertySymbol in propertiesOfClass)
        {
            // skip adding to command property if it is not id of the entity
            var propertyNameLower = propertySymbol.Name.ToLower();
            if (!propertyNameLower.Equals("id") && !propertyNameLower.Equals($"{symbol.Name}id"))
            {
                continue;
            }

            result.Add($"{fromObject}{propertySymbol.Name}");
        }

        return result;
    }
}