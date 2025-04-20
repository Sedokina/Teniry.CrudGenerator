using Teniry.CrudGenerator.Tests.Helpers;

namespace Teniry.CrudGenerator.Tests;

public class GetByIdCrudGeneratorTests {
    private readonly SutBuilder _sutBuilder = SutBuilder.Default()
        .WithGetByIdConfiguration(
            """
            GetByIdOperation = new() {
                Generate = true
            };
            """
        );

    [Fact]
    public Task Should_NotGenerateFiles_When_GenerateIsFalse() {
        var source = _sutBuilder.WithGetByIdConfiguration(
            """
            GetByIdOperation = new() {
                Generate = false
            };
            """
        ).Build();

        return CrudHelper.Verify(source);
    }

    [Fact]
    public Task Should_NotGenerateEndpointFile_When_GenerateEndpointIsFalse() {
        var source = _sutBuilder
            .WithGetByIdConfiguration(
                """
                GetByIdOperation = new() {
                    GenerateEndpoint = false
                };
                """
            ).Build();

        return CrudHelper.Verify(source)
            .IgnoreGeneratedResult(x => !x.HintName.Equals("GetTestEntityEndpoint.g.cs"));
    }

    [Fact]
    public Task Should_GenerateClassNamesWithNewOperationName() {
        var source = _sutBuilder
            .WithGetByIdConfiguration(
                """
                GetByIdOperation = new() {
                    Operation = "Fetch"
                };
                """
            ).Build();

        return CrudHelper.Verify(source);
    }

    [Fact]
    public Task Should_GenerateFullyCustomizedClassNames() {
        var source = SutBuilder.Default()
            .WithGetByIdConfiguration(
                """
                GetByIdOperation = new() {
                    OperationGroup = "FetchCustomNs",
                    QueryName = "FetchEntityCustomCommand",
                    HandlerName = "FetchEntityCustomHandler",
                    DtoName = "FetchCustomDto",
                    EndpointClassName = "FetchCustomEndpoint",
                    EndpointFunctionName = "RunFetchAsync",
                    RouteName = "/customGet/{{id_param_name}}"
                };
                """
            ).Build();

        return CrudHelper.Verify(source);
    }
}