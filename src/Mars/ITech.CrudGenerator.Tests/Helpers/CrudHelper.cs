using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace ITech.CrudGenerator.Tests.Helpers;

internal class CrudHelper
{
    public static (ImmutableArray<Diagnostic> Diagnostics, string[] Output) RunGenerator(string source)
    {
        return TestHelpers.GetGeneratedTrees<CrudGenerator>([source],
            GetTrackingNamesOf(typeof(CrudGeneratorTrackingNames)));
    }

    private static string[] GetTrackingNamesOf(Type type)
    {
        return type
            .GetFields()
            .Where(x => x is { IsLiteral: true, IsInitOnly: false } && x.FieldType == typeof(string))
            .Select(x => (string)x.GetRawConstantValue()!)
            .Where(x => !string.IsNullOrEmpty(x))
            .ToArray();
    }
}