using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Teniry.CrudGenerator.Tests.Helpers;

internal static class CrudHelper {
    private static readonly VerifySettings Settings;

    static CrudHelper() {
        Settings = new();
        Settings.UseDirectory("snapshots");
    }

    public static (ImmutableArray<Diagnostic> Diagnostics, string[] Output) RunGeneratorIncrementaly(string source) {
        return TestHelpers.GetGeneratedTrees<CrudGenerator>(
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

    public static SettingsTask Verify(string source) {
        var compilation = TestHelpers.CreateCompilation<CrudGenerator>([source]);
        var generator = new CrudGenerator();

        // The GeneratorDriver is used to run generator against a compilation
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the source generator
        driver = driver.RunGenerators(compilation);

        return Verifier.Verify(driver, Settings);
    }
}