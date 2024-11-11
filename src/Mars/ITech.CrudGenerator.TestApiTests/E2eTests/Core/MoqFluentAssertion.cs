using FluentAssertions.Execution;
using Xunit.Abstractions;

namespace ITech.CrudGenerator.TestApiTests.E2eTests.Core;

public class MoqFluentAssertion<T> {
    private readonly Action<T>          _fluentAssertionsChecker;
    private readonly ITestOutputHelper? _testOutputHelper;

    public MoqFluentAssertion(Action<T> fluentAssertionsChecker, ITestOutputHelper? testOutputHelper) {
        _fluentAssertionsChecker = fluentAssertionsChecker;
        _testOutputHelper        = testOutputHelper;
    }

    public bool Test(T valueToCheck) {
        using var assertionScope = new AssertionScope();
        _fluentAssertionsChecker(valueToCheck);
        var errors = assertionScope.Discard();

        if (!errors.Any()) return true;

        _testOutputHelper?.WriteLine("---   Assertions that did not meet requirements   ---");
        _testOutputHelper?.WriteLine(string.Join("\n", errors));

        return false;
    }
}