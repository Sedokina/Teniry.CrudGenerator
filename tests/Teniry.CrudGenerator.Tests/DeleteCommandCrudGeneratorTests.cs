using Teniry.CrudGenerator.Tests.Helpers;

namespace Teniry.CrudGenerator.Tests;

public class DeleteCommandCrudGeneratorTests {
    private readonly SutBuilder _sutBuilder = SutBuilder.Default()
        .WithDeleteConfiguration(
            """
            DeleteOperation = new() {
                Generate = true
            };
            """
        );

    [Fact]
    public Task Should_NotGenerateFiles_When_GenerateIsFalse() {
        var source = _sutBuilder.WithDeleteConfiguration(
            """
            DeleteOperation = new() {
                Generate = false
            };
            """
        ).Build();

        return CrudHelper.Verify(source);
    }

    [Fact]
    public Task Should_NotGenerateEndpointFile_When_GenerateEndpointIsFalse() {
        var source = _sutBuilder
            .WithDeleteConfiguration(
                """
                DeleteOperation = new() {
                    GenerateEndpoint = false
                };
                """
            ).Build();

        return CrudHelper.Verify(source)
            .IgnoreGeneratedResult(x => !x.HintName.Equals("DeleteTestEntityEndpoint.g.cs"));
    }

    [Fact]
    public Task Should_GenerateClassNamesWithNewOperationName() {
        var source = _sutBuilder
            .WithDeleteConfiguration(
                """
                DeleteOperation = new() {
                    Operation = "Del"
                };
                """
            ).Build();

        return CrudHelper.Verify(source);
    }

    [Fact]
    public Task Should_GenerateFullyCustomizedClassNames() {
        var source = SutBuilder.Default()
            .WithDeleteConfiguration(
                """
                DeleteOperation = new() {
                    OperationGroup = "DelCustomNs",
                    CommandName = "DelEntityCustomCommand",
                    HandlerName = "DelEntityCustomHandler",
                    EndpointClassName = "DelCustomEndpoint",
                    EndpointFunctionName = "RunDelAsync",
                    RouteName = "/customDelete/{{entity_name}}/{{id_param_name}}"
                };
                """
            ).Build();

        return CrudHelper.Verify(source);
    }
}