using Teniry.CrudGenerator.Tests.Helpers;

namespace Teniry.CrudGenerator.Tests;

public class CreateCommandCrudGeneratorTests {
    private readonly SutBuilder _sutBuilder = SutBuilder.Default()
        .WithCreateConfiguration(
            """
            CreateOperation = new() {
                Generate = true
            };
            """
        );

    [Fact]
    public Task Should_NotGenerateFiles_When_GenerateIsFalse() {
        var source = _sutBuilder.WithCreateConfiguration(
            """
            CreateOperation = new() {
                Generate = false
            };
            """
        ).Build();

        return CrudHelper.Verify(source);
    }

    [Fact]
    public Task Should_NotGenerateEndpointFile_When_GenerateEndpointIsFalse() {
        var source = _sutBuilder
            .WithCreateConfiguration(
                """
                CreateOperation = new() {
                    GenerateEndpoint = false
                };
                """
            ).Build();

        return CrudHelper.Verify(source)
            .IgnoreGeneratedResult(x => !x.HintName.Equals("CreateTestEntityEndpoint.g.cs"));
    }

    [Fact]
    public Task Should_GenerateClassNamesWithNewOperationName() {
        var source = _sutBuilder
            .WithCreateConfiguration(
                """
                CreateOperation = new() {
                    Operation = "Add"
                };
                """
            ).Build();

        return CrudHelper.Verify(source);
    }

    [Fact]
    public Task Should_GenerateFullyCustomizedClassNames() {
        var source = SutBuilder.Default()
            .WithCreateConfiguration(
                """
                CreateOperation = new() {
                    OperationGroup = "CreateCustomNs",
                    CommandName = "CreateEntityCustomCommand",
                    HandlerName = "CreateEntityCustomHandler",
                    DtoName = "CreatedCustomDto",
                    EndpointClassName = "CreatedCustomEndpoint",
                    EndpointFunctionName = "RunCreateAsync",
                    RouteName = "/customizedCreate"
                };
                """
            ).Build();

        return CrudHelper.Verify(source);
    }

    [Fact]
    public Task Should_ReturnLocationToGetEntityFromCreateEndpoint() {
        var source = _sutBuilder
            .WithGetByIdConfiguration(
                """
                GetByIdOperation = new() {
                   Generate = true
                };
                """
            ).Build();

        return CrudHelper.Verify(source)
            .IgnoreGeneratedResult(x => !x.HintName.Equals("CreateTestEntityEndpoint.g.cs"));
    }

    [Fact]
    public Task Should_NotReturnLocationToGetEntityFromCreateEndpoint_When_GetOperationNotGenerated() {
        var source = _sutBuilder.Build();

        return CrudHelper.Verify(source)
            .IgnoreGeneratedResult(x => !x.HintName.Equals("CreateTestEntityEndpoint.g.cs"));
    }

    [Fact]
    public Task Should_NotReturnLocationToGetEntityFromCreateEndpoint_When_GetOperationEndpointNotGenerated() {
        var source = _sutBuilder.WithGetByIdConfiguration(
            """
            GetByIdOperation = new() {
               GenerateEndpoint = false
            };
            """
        ).Build();

        return CrudHelper.Verify(source)
            .IgnoreGeneratedResult(x => !x.HintName.Equals("CreateTestEntityEndpoint.g.cs"));
    }
}