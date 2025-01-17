using System.Collections.Immutable;
using Teniry.CrudGenerator;
using Microsoft.CodeAnalysis;

namespace Teniry.CrudGenerator.Tests.Helpers;

internal class CrudHelper {
    public static (ImmutableArray<Diagnostic> Diagnostics, string[] Output) RunGenerator(string source) {
        return TestHelpers.GetGeneratedTrees<Teniry.CrudGenerator.CrudGenerator>(
            [source],
            GetTrackingNamesOf(typeof(CrudGeneratorTrackingNames))
        );
    }

    private static string[] GetTrackingNamesOf(Type type) {
        return type
            .GetFields()
            .Where(x => x is { IsLiteral: true, IsInitOnly: false } && x.FieldType == typeof(string))
            .Select(x => (string)x.GetRawConstantValue()!)
            .Where(x => !string.IsNullOrEmpty(x))
            .ToArray();
    }
}