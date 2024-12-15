using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace ITech.CrudGenerator.TestApiTests.E2eTests.Core;

public static class FluentAssertionsExtensions
{
    public static AndConstraint<HttpResponseMessageAssertions> FailIfNotSuccessful(
        this HttpResponseMessageAssertions httpResponseMessageAssertions,
        string because = "",
        params object[] becauseArgs
    )
    {
        var success = Execute.Assertion
            .ForCondition(httpResponseMessageAssertions.Subject is not null)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected HttpStatusCode to be successful (2xx){reason}, but HttpResponseMessage was <null>.");

        if (success)
            Execute.Assertion
                .ForCondition(httpResponseMessageAssertions.Subject!.IsSuccessStatusCode)
                .BecauseOf(because, becauseArgs)
                .FailWith(
                    () =>
                    {
                        var content = httpResponseMessageAssertions.Subject
                            .Content
                            .ReadAsStringAsync()
                            .GetAwaiter()
                            .GetResult();

                        return new FailReason(
                            "Expected HttpStatusCode to be successful (2xx){reason}, but found {0}. {1}",
                            httpResponseMessageAssertions.Subject.StatusCode,
                            content
                        );
                    }
                );

        return new AndConstraint<HttpResponseMessageAssertions>(httpResponseMessageAssertions);
    }
}