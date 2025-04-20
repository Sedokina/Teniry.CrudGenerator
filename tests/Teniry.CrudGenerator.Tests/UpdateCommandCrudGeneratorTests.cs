using Teniry.CrudGenerator.Tests.Helpers;

namespace Teniry.CrudGenerator.Tests;

public class UpdateCommandCrudGeneratorTests {
    private readonly SutBuilder _sutBuilder = SutBuilder.Default()
        .WithUpdateConfiguration(
            """
            UpdateOperation = new() {
                Generate = true
            };
            """
        );

    [Fact]
    public Task Should_NotGenerateFiles_When_GenerateIsFalse() {
        var source = _sutBuilder.WithUpdateConfiguration(
            """
            UpdateOperation = new() {
                Generate = false
            };
            """
        ).Build();

        return CrudHelper.Verify(source);
    }

    [Fact]
    public Task Should_NotGenerateEndpointFile_When_GenerateEndpointIsFalse() {
        var source = _sutBuilder
            .WithUpdateConfiguration(
                """
                UpdateOperation = new() {
                    GenerateEndpoint = false
                };
                """
            ).Build();

        return CrudHelper.Verify(source)
            .IgnoreGeneratedResult(x => !x.HintName.Equals("UpdateTestEntityEndpoint.g.cs"));
    }

    [Fact]
    public Task Should_GenerateClassNamesWithNewOperationName() {
        var source = _sutBuilder
            .WithUpdateConfiguration(
                """
                UpdateOperation = new() {
                    Operation = "Upd"
                };
                """
            ).Build();

        return CrudHelper.Verify(source);
    }

    [Fact]
    public Task Should_GenerateFullyCustomizedClassNames() {
        var source = SutBuilder.Default()
            .WithUpdateConfiguration(
                """
                UpdateOperation = new() {
                    OperationGroup = "UpdCustomNs",
                    CommandName = "UpdEntityCustomCommand",
                    HandlerName = "UpdEntityCustomHandler",
                    ViewModelName = "UpdCustomVm",
                    EndpointClassName = "UpdCustomEndpoint",
                    EndpointFunctionName = "RunUpdAsync",
                    RouteName = "/customizedUpdate/{{id_param_name}}"
                };
                """
            ).Build();

        return CrudHelper.Verify(source);
    }
}