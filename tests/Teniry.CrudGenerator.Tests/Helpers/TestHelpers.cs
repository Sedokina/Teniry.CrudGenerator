using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using Teniry.CrudGenerator.Abstractions.DbContext;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Teniry.CrudGenerator.Tests.Helpers;

/// <summary>
/// This test helper allows to run a source generator and verify that the generator steps are cached correctly
/// </summary>
/// <remarks>It was taken from https://andrewlock.net/creating-a-source-generator-part-10-testing-your-incremental-generator-pipeline-outputs-are-cacheable/</remarks>
public static class TestHelpers {
    public static CSharpCompilation CreateCompilation<T>(
        string[] sources // C# source code 
    ) {
        // Convert the source files to SyntaxTrees
        var syntaxTrees = sources.Select(static x => CSharpSyntaxTree.ParseText(x));

        // Configure the assembly references you need
        // This will vary depending on your generator and requirements
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location))
            .Select(x => MetadataReference.CreateFromFile(x.Location))
            .Concat(
                [
                    MetadataReference.CreateFromFile(typeof(T).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(DbContextDbProvider).Assembly.Location)
                ]
            );

        // Create a Compilation object
        // You may want to specify other results here
        return CSharpCompilation.Create(
            Assembly.GetExecutingAssembly().GetName().Name,
            syntaxTrees,
            references,
            new(OutputKind.DynamicallyLinkedLibrary)
        );
    }

    // You call this method passing in C# sources, and the list of stages you expect
    // It runs the generator, asserts the outputs are ok, 
    public static (ImmutableArray<Diagnostic> Diagnostics, string[] Output) GetGeneratedTrees<T>(
        string[] sources, // C# source code 
        string[] stages, // The tracking stages we expect
        bool assertOutputs = true
    ) // You can disable cacheability checking during dev
        where T : IIncrementalGenerator, new() // T is your generator
    {
        var compilation = CreateCompilation<T>(sources);

        // Run the generator, get the results, and assert cacheability if applicable
        var runResult = RunGeneratorAndAssertOutput<T>(compilation, stages, assertOutputs);

        // Return the generator diagnostics and generated sources
        return (runResult.Diagnostics, runResult.GeneratedTrees.Select(x => x.ToString()).ToArray());
    }

    private static GeneratorDriverRunResult RunGeneratorAndAssertOutput<T>(
        CSharpCompilation compilation,
        string[] trackingNames,
        bool assertOutput = true
    )
        where T : IIncrementalGenerator, new() {
        var generator = new T().AsSourceGenerator();

        // ⚠ Tell the driver to track all the incremental generator outputs
        // without this, you'll have no tracked outputs!
        var opts = new GeneratorDriverOptions(
            IncrementalGeneratorOutputKind.None,
            true
        );

        GeneratorDriver driver = CSharpGeneratorDriver.Create([generator], driverOptions: opts);

        // Create a clone of the compilation that we will use later
        var clone = compilation.Clone();

        // Do the initial run
        // Note that we store the returned driver value, as it contains cached previous outputs
        driver = driver.RunGenerators(compilation);
        var runResult = driver.GetRunResult();

        if (assertOutput) {
            // Run again, using the same driver, with a clone of the compilation
            var runResult2 = driver
                .RunGenerators(clone)
                .GetRunResult();

            // Compare all the tracked outputs, throw if there's a failure
            AssertRunsEqual(runResult, runResult2, trackingNames);

            // verify the second run only generated cached source outputs
            runResult2.Results[0]
                .TrackedOutputSteps
                .SelectMany(x => x.Value) // step executions
                .SelectMany(x => x.Outputs) // execution results
                .Should()
                .OnlyContain(x => x.Reason == IncrementalStepRunReason.Cached);
        }

        return runResult;
    }

    private static void AssertRunsEqual(
        GeneratorDriverRunResult runResult1,
        GeneratorDriverRunResult runResult2,
        string[] trackingNames
    ) {
        // We're given all the tracking names, but not all the
        // stages will necessarily execute, so extract all the 
        // output steps, and filter to ones we know about
        var trackedSteps1 = GetTrackedSteps(runResult1, trackingNames);
        var trackedSteps2 = GetTrackedSteps(runResult2, trackingNames);

        // Both runs should have the same tracked steps
        trackedSteps1.Should()
            .NotBeEmpty()
            .And.HaveSameCount(trackedSteps2)
            .And.ContainKeys(trackedSteps2.Keys);

        // Get the IncrementalGeneratorRunStep collection for each run
        foreach (var (trackingName, runSteps1) in trackedSteps1) {
            // Assert that both runs produced the same outputs
            var runSteps2 = trackedSteps2[trackingName];
            AssertEqual(runSteps1, runSteps2, trackingName);
        }

        return;

        // Local function that extracts the tracked steps
        static Dictionary<string, ImmutableArray<IncrementalGeneratorRunStep>> GetTrackedSteps(
            GeneratorDriverRunResult runResult,
            string[] trackingNames
        ) {
            return runResult
                .Results[0] // We're only running a single generator, so this is safe
                .TrackedSteps // Get the pipeline outputs
                .Where(step => trackingNames.Contains(step.Key)) // filter to known steps
                .ToDictionary(x => x.Key, x => x.Value);
        } // Convert to a dictionary
    }

    private static void AssertEqual(
        ImmutableArray<IncrementalGeneratorRunStep> runSteps1,
        ImmutableArray<IncrementalGeneratorRunStep> runSteps2,
        string stepName
    ) {
        runSteps1.Should().HaveSameCount(runSteps2);

        for (var i = 0; i < runSteps1.Length; i++) {
            var runStep1 = runSteps1[i];
            var runStep2 = runSteps2[i];

            // The outputs should be equal between different runs
            IEnumerable<object> outputs1 = runStep1.Outputs.Select(x => x.Value);
            IEnumerable<object> outputs2 = runStep2.Outputs.Select(x => x.Value);

            outputs1.Should()
                .Equal(outputs2, $"because {stepName} should produce cacheable outputs");

            // Therefore, on the second run the results should always be cached or unchanged!
            // - Unchanged is when the _input_ has changed, but the output hasn't
            // - Cached is when the the input has not changed, so the cached output is used 
            runStep2.Outputs.Should()
                .OnlyContain(
                    x => x.Reason == IncrementalStepRunReason.Cached || x.Reason == IncrementalStepRunReason.Unchanged,
                    $"{stepName} expected to have reason {IncrementalStepRunReason.Cached} or {
                        IncrementalStepRunReason.Unchanged}"
                );

            // Make sure we're not using anything we shouldn't
            AssertObjectGraph(runStep1, stepName);
        }
    }

    private static void AssertObjectGraph(IncrementalGeneratorRunStep runStep, string stepName) {
        // Including the stepName in error messages to make it easy to isolate issues
        var because = $"{stepName} shouldn't contain banned symbols";
        var visited = new HashSet<object>();

        // Check all of the outputs - probably overkill, but why not
        foreach (var (obj, _) in runStep.Outputs) {
            Visit(obj);
        }

        void Visit(object? node) {
            // If we've already seen this object, or it's null, stop.
            if (node is null || !visited.Add(node)) {
                return;
            }

            // Make sure it's not a banned type
            node.Should()
                .NotBeOfType<Compilation>(because)
                .And.NotBeOfType<ISymbol>(because)
                .And.NotBeOfType<SyntaxNode>(because);

            // Examine the object
            var type = node.GetType();
            if (type.IsPrimitive || type.IsEnum || type == typeof(string)) {
                return;
            }

            // If the object is a collection, check each of the values
            if (node is IEnumerable collection and not string) {
                foreach (var element in collection) {
                    // recursively check each element in the collection
                    Visit(element);
                }

                return;
            }

            // Recursively check each field in the object
            foreach (var field in type.GetFields(
                         BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                     )) {
                var fieldValue = field.GetValue(node);
                Visit(fieldValue);
            }
        }
    }
}