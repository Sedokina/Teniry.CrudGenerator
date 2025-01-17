using Moq;
using Xunit.Abstractions;

namespace ITech.CrudGenerator.TestApiTests.E2eTests.Core;

public static class Should {
    public static T Assert<T>(MoqFluentAssertion<T> moqFluentAssertion) {
        return It.Is<T>(c => moqFluentAssertion.Test(c));
    }

    public static MoqFluentAssertion<T> Assert<T>(
        Action<T> fluentAssertionChecks,
        ITestOutputHelper? testOutputHelper = null
    ) {
        return new(fluentAssertionChecks, testOutputHelper);
    }
}