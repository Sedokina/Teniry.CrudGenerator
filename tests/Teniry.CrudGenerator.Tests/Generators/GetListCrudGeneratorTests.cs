using Teniry.CrudGenerator.Tests.Helpers;

namespace Teniry.CrudGenerator.Tests.Generators;

public class GetListCrudGeneratorTests {
    private readonly SutBuilder _sutBuilder = SutBuilder.Default()
        .WithGetListConfiguration(
            """
            GetListOperation = new() {
                Generate = true
            };
            """
        );

    [Fact]
    public Task Should_NotGenerateFiles_When_GenerateIsFalse() {
        var source = _sutBuilder.WithGetListConfiguration(
            """
            GetListOperation = new() {
                Generate = false
            };
            """
        ).Build();

        return CrudHelper.Verify(source);
    }

    [Fact]
    public Task Should_NotGenerateEndpointFile_When_GenerateEndpointIsFalse() {
        var source = _sutBuilder
            .WithGetListConfiguration(
                """
                GetListOperation = new() {
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
            .WithGetListConfiguration(
                """
                GetListOperation = new() {
                    Operation = "Fetch"
                };
                """
            ).Build();

        return CrudHelper.Verify(source);
    }

    [Fact]
    public Task Should_GenerateFullyCustomizedClassNames() {
        var source = SutBuilder.Default()
            .WithGetListConfiguration(
                """
                GetListOperation = new() {
                    OperationGroup = "FetchCustomNs",
                    QueryName = "FetchEntityCustomCommand",
                    HandlerName = "FetchEntityCustomHandler",
                    DtoName = "FetchCustomDto",
                    ListItemDtoName = "CustomFetchListItemDto",
                    EndpointClassName = "FetchCustomEndpoint",
                    EndpointFunctionName = "RunFetchAsync",
                    RouteName = "/fetchAll"
                };
                """
            ).Build();

        return CrudHelper.Verify(source);
    }
}