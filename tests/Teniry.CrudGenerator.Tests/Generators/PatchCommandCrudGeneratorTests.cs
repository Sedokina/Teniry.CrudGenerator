using Teniry.CrudGenerator.Tests.Helpers;

namespace Teniry.CrudGenerator.Tests.Generators;

public class PatchCommandCrudGeneratorTests {
    private readonly SutBuilder _sutBuilder = SutBuilder.Default()
        .WithPatchConfiguration(
            """
            PatchOperation = new() {
                Generate = true
            };
            """
        );

    [Fact]
    public Task Should_NotGenerateFiles_When_GenerateIsFalse() {
        var source = _sutBuilder.WithPatchConfiguration(
            """
            PatchOperation = new() {
                Generate = false
            };
            """
        ).Build();

        return CrudHelper.Verify(source);
    }

    [Fact]
    public Task Should_NotGenerateEndpointFile_When_GenerateEndpointIsFalse() {
        var source = _sutBuilder
            .WithPatchConfiguration(
                """
                PatchOperation = new() {
                    GenerateEndpoint = false
                };
                """
            ).Build();

        return CrudHelper.Verify(source)
            .IgnoreGeneratedResult(x => !x.HintName.Equals("PatchTestEntityEndpoint.g.cs"));
    }

    [Fact]
    public Task Should_GenerateClassNamesWithNewOperationName() {
        var source = _sutBuilder
            .WithPatchConfiguration(
                """
                PatchOperation = new() {
                    Operation = "Upd"
                };
                """
            ).Build();

        return CrudHelper.Verify(source);
    }

    [Fact]
    public Task Should_GenerateFullyCustomizedClassNames() {
        var source = SutBuilder.Default()
            .WithPatchConfiguration(
                """
                PatchOperation = new() {
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

    [Fact]
    public Task Should_GenerateResetForField() {
        var source = _sutBuilder.WithEntity(
            "TestEntity",
            """
            public class TestEntity {
                   public int Id { get; set; }
                   public string? ResetableName { get; set; }
            }
            """
        ).Build();

        return CrudHelper.Verify(source);
    }
}