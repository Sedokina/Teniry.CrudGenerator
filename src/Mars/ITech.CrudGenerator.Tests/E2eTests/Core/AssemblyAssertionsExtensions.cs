using FluentAssertions.Execution;
using FluentAssertions.Reflection;

namespace ITech.CrudGenerator.Tests.E2eTests.Core;

public static class AssemblyAssertionsExtensions
{
    public static AndConstraint<AssemblyAssertions> ContainType(this AssemblyAssertions parent,
        string typeName, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(!string.IsNullOrEmpty(typeName))
            .FailWith("You can't assert a type exist if you don't pass a proper name")
            .Then
            .Given(() => parent.Subject.GetTypes())
            .ForCondition(types => types.Any(type => type.Name.Equals(typeName)))
            .FailWith("Expected {context:directory} to contain {0}{reason}, but found {1}.",
                _ => typeName, types => types.Select(type => type.Name));

        return new AndConstraint<AssemblyAssertions>(parent);
    }

    [CustomAssertion]
    public static AndConstraint<AssemblyAssertions> NotContainType(this AssemblyAssertions parent,
        string typeName, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(!string.IsNullOrEmpty(typeName))
            .FailWith("You can't assert a type exist if you don't pass a proper name")
            .Then
            .Given(() => parent.Subject.GetTypes())
            .ForCondition(types => types.All(type => !type.Name.Equals(typeName)))
            .FailWith("Expected {context:directory} not to contain {0}{reason}, but found {1}.",
                _ => typeName, types => types.Where(type => type.Name.Equals(typeName)).Select(type => type.FullName));

        return new AndConstraint<AssemblyAssertions>(parent);
    }

    [CustomAssertion]
    public static AndConstraint<AssemblyAssertions> BeInNamespaceThatEndsWith(this AssemblyAssertions parent,
        string typeName, string endsWith, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(!string.IsNullOrEmpty(typeName) && !string.IsNullOrEmpty(endsWith))
            .FailWith("You can't assert a type in namespace if you don't pass a proper name")
            .Then
            .Given(() => parent.Subject.GetTypes().Where(x => x.Name.Equals(typeName)))
            .ForCondition(types => types.All(type => type.Namespace != null && type.Namespace.EndsWith(endsWith)))
            .FailWith("Expected {context:directory} type {0} namespace end with {1}{reason}, but found {2}.",
                _ => typeName,
                _ => endsWith,
                types => types.Select(type => type.FullName));

        return new AndConstraint<AssemblyAssertions>(parent);
    }
}