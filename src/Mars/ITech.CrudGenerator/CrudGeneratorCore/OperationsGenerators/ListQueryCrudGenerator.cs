using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators;

internal class ListQueryCrudGenerator : BaseOperationCrudGenerator<CqrsListOperationGeneratorConfiguration>
{
    private readonly string _dtoName;
    private readonly string _handlerName;
    private readonly string _listItemDtoName;
    private readonly string _queryName;
    private readonly string _endpointClassName;
    private readonly string _filterName;

    public ListQueryCrudGenerator(CrudGeneratorScheme<CqrsListOperationGeneratorConfiguration> scheme) : base(scheme)
    {
        _queryName = Scheme.Configuration.Operation;
        _listItemDtoName = Scheme.Configuration.DtoListItem;
        _dtoName = Scheme.Configuration.Dto;
        _filterName = Scheme.Configuration.Filter;
        _handlerName = Scheme.Configuration.Handler;
        _endpointClassName = Scheme.Configuration.Endpoint.Name;
    }

    public override void RunGenerator()
    {
        GenerateQuery();
        GenerateListItemDto();
        GenerateDto();
        GenerateFilter();
        GenerateHandler();
        if (Scheme.Configuration.Endpoint.Generate)
        {
            GenerateEndpoint();
        }
    }

    private void GenerateQuery()
    {
        var query = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.PartialKeyword
            ], _queryName)
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation)
            .WithUsings([
                "ITech.Cqrs.Queryables.Page",
                "ITech.Cqrs.Queryables.Sort"
            ])
            .Implements("IDefineSortable")
            .Implements("IPage")
            .WithXmlDoc($"Get {EntityScheme.EntityTitle.PluralTitle}",
                $"Returns {EntityScheme.EntityTitle.PluralTitle} of type <see cref=\"{_dtoName}\" />");

        foreach (var property in EntityScheme.Properties)
        {
            if (property.FilterProperties.Length <= 0) continue;
            foreach (var filterProperty in property.FilterProperties)
            {
                query.WithProperty(filterProperty.TypeName, filterProperty.PropertyName);
            }
        }

        query.WithProperty("int", "Page").WithInheritDoc();
        query.WithProperty("int", "PageSize").WithInheritDoc();
        query.WithProperty("string[]?", "Sort").WithInheritDoc();

        var method = new MethodBuilder([SyntaxKind.PublicKeyword], "string[]", "GetSortKeys")
            .WithXmlInheritdoc();
        var methodBody = new MethodBodyBuilder()
            .InitStringArray("result", EntityScheme.SortableProperties.Select(x => x.SortKey).ToArray())
            .ReturnVariable("result");

        method.WithBody(methodBody.Build());
        query.WithMethod(method.Build());

        WriteFile(_queryName, query.BuildAsString());
    }

    private void GenerateListItemDto()
    {
        var dtoClass = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.PartialKeyword
            ], _listItemDtoName)
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation);

        foreach (var property in EntityScheme.Properties)
        {
            dtoClass.WithProperty(property.TypeName, property.PropertyName)
                .WithDefaultValue(property.DefaultValue);
        }

        WriteFile(_listItemDtoName, dtoClass.BuildAsString());
    }

    private void GenerateDto()
    {
        var dtoClass = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.PartialKeyword
            ], _dtoName)
            .WithUsings(["ITech.Cqrs.Queryables.Page"])
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation)
            .Implements("PagedResult", _listItemDtoName);

        var constructor = new ConstructorBuilder([SyntaxKind.PublicKeyword], _dtoName)
            .WithParameters([
                new ParameterOfMethodBuilder($"List<{_listItemDtoName}>", "items"),
                new ParameterOfMethodBuilder("PageInfo", "page")
            ])
            .WithBaseConstructor(["items", "page"]);

        constructor.WithBody(new MethodBodyBuilder().Build());
        dtoClass.WithConstructor(constructor.Build());

        WriteFile(_dtoName, dtoClass.BuildAsString());
    }

    private void GenerateFilter()
    {
        var query = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.PartialKeyword
            ], _filterName)
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation)
            .WithUsings([
                "System.Linq.Expressions",
                "Microsoft.EntityFrameworkCore",
                "ITech.Cqrs.Queryables.Filter",
                Scheme.EntityScheme.EntityNamespace
            ])
            .Implements("QueryableFilter", EntityScheme.EntityName.ToString());

        foreach (var property in EntityScheme.Properties)
        {
            if (property.FilterProperties.Length <= 0) continue;
            foreach (var filterProperty in property.FilterProperties)
            {
                query.WithProperty(filterProperty.TypeName, filterProperty.PropertyName);
            }
        }

        query.WithMethod(CreateSortMethodForFilter().Build());
        query.WithMethod(CreateDefaultSortMethod().Build());
        query.WithMethod(CreateFilterMethod().Build());

        WriteFile(_filterName, query.BuildAsString());
    }

    private MethodBuilder CreateFilterMethod()
    {
        var filterMethod = new MethodBuilder([SyntaxKind.ProtectedKeyword, SyntaxKind.OverrideKeyword],
                $"IQueryable<{Scheme.EntityScheme.EntityName}>", "Filter")
            .WithParameters([new ParameterOfMethodBuilder($"IQueryable<{Scheme.EntityScheme.EntityName}>", "query")])
            .WithXmlInheritdoc();

        var filterBody = new MethodBodyBuilder();

        foreach (var property in EntityScheme.Properties)
        {
            foreach (var filterProperty in property.FilterProperties)
            {
                var expression = filterProperty.FilterExpression
                    .BuildExpression(filterProperty.PropertyName, property.PropertyName);
                filterBody.AddExpression(expression);
            }
        }

        filterBody.ReturnVariable("query");

        filterMethod.WithBody(filterBody.Build());
        return filterMethod;
    }

    private MethodBuilder CreateSortMethodForFilter()
    {
        var method = new MethodBuilder([SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword],
                $"Dictionary<string, Expression<Func<{Scheme.EntityScheme.EntityName}, object>>>", "Sort")
            .WithXmlInheritdoc();

        var dictionaryInitializationArgumets = new List<SyntaxNodeOrToken>();

        foreach (var sortableProperty in EntityScheme.SortableProperties)
        {
            dictionaryInitializationArgumets.Add(SyntaxFactory.InitializerExpression(
                SyntaxKind.ComplexElementInitializerExpression,
                SyntaxFactory.SeparatedList<ExpressionSyntax>(
                    new SyntaxNodeOrToken[]
                    {
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            SyntaxFactory.Literal(sortableProperty.SortKey)),
                        SyntaxFactory.Token(SyntaxKind.CommaToken),
                        SyntaxFactory.SimpleLambdaExpression(
                                SyntaxFactory.Parameter(
                                    SyntaxFactory.Identifier("x")))
                            .WithExpressionBody(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName("x"),
                                    SyntaxFactory.IdentifierName(sortableProperty.PropertyName)))
                    })));
            dictionaryInitializationArgumets.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
        }

        var resultDictionary = SyntaxFactory.ObjectCreationExpression(
                SyntaxFactory.ParseTypeName(
                    $"Dictionary<string, Expression<Func<{Scheme.EntityScheme.EntityName}, object>>>"))
            .WithInitializer(
                SyntaxFactory.InitializerExpression(
                    SyntaxKind.CollectionInitializerExpression,
                    SyntaxFactory.SeparatedList<ExpressionSyntax>(dictionaryInitializationArgumets.ToArray())));

        var sortMethodBody = SyntaxFactory.Block(
            SyntaxFactory.ReturnStatement(resultDictionary)
        );

        method.WithBody(sortMethodBody);
        return method;
    }

    private MethodBuilder CreateDefaultSortMethod()
    {
        var method = new MethodBuilder([SyntaxKind.ProtectedKeyword, SyntaxKind.OverrideKeyword],
                $"IQueryable<{Scheme.EntityScheme.EntityName}>", "DefaultSort")
            .WithParameters([new ParameterOfMethodBuilder($"IQueryable<{Scheme.EntityScheme.EntityName}>", "query")])
            .WithXmlInheritdoc();

        BlockSyntax? defaultSortBody;
        if (EntityScheme.DefaultSort is null)
        {
            defaultSortBody = SyntaxFactory.Block(SyntaxFactory.ReturnStatement(
                SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.BaseExpression(),
                            SyntaxFactory.IdentifierName("DefaultSort")))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.IdentifierName("query")))))));
        }
        else
        {
            defaultSortBody = SyntaxFactory.Block(SyntaxFactory.ReturnStatement(SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("query"),
                        SyntaxFactory.IdentifierName(EntityScheme.DefaultSort.Direction.Equals("asc")
                            ? "OrderBy"
                            : "OrderByDescending")))
                .WithArgumentList(
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Argument(
                                SyntaxFactory.SimpleLambdaExpression(
                                        SyntaxFactory.Parameter(
                                            SyntaxFactory.Identifier("x")))
                                    .WithExpressionBody(
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.IdentifierName("x"),
                                            SyntaxFactory.IdentifierName(EntityScheme.DefaultSort.PropertyName)))))))));
        }

        method.WithBody(defaultSortBody);
        return method;
    }

    private void GenerateHandler()
    {
        var handlerClass = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.PartialKeyword
            ], _handlerName)
            .WithUsings([
                "Microsoft.EntityFrameworkCore",
                "ITech.Cqrs.Cqrs.Queries",
                "ITech.Cqrs.Domain.Exceptions",
                "ITech.Cqrs.Queryables.Page",
                "ITech.Cqrs.Queryables.Filter",
                Scheme.DbContextScheme.DbContextNamespace,
                EntityScheme.EntityNamespace,
                "Mapster"
            ])
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation)
            .Implements("IQueryHandler", _queryName, _dtoName)
            .WithPrivateField([SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword],
                Scheme.DbContextScheme.DbContextName, "_db");

        var constructor = new ConstructorBuilder([SyntaxKind.PublicKeyword], _handlerName)
            .WithParameters([new ParameterOfMethodBuilder(Scheme.DbContextScheme.DbContextName, "db")]);
        var constructorBody = new MethodBodyBuilder()
            .AssignVariable("_db", "db");

        constructor.WithBody(constructorBody.Build());

        var methodBuilder = new MethodBuilder([
                    SyntaxKind.PublicKeyword,
                    SyntaxKind.AsyncKeyword
                ], $"Task<{_dtoName}>", "HandleAsync")
            .WithParameters([
                new ParameterOfMethodBuilder(_queryName, "query"),
                new ParameterOfMethodBuilder(nameof(CancellationToken), "cancellation")
            ])
            .WithXmlInheritdoc();

        var methodBodyBuilder = new MethodBodyBuilder()
            .InitVariableFromGenericMethodCall("filter", "query", "Adapt", [_filterName], [])
            .AssignVariable("filter.Sorts", "query.Sort")
            .InitVariableFromAsyncMethodCall("items", linqBuilder =>
            {
                linqBuilder.CallGenericMethod("_db", "Set", [Scheme.EntityScheme.EntityName.ToString()], [])
                    .ThenMethod("Filter", ["filter"])
                    .ThenGenericMethod("ProjectToType", [_listItemDtoName], [])
                    .ThenMethod("ToPagedListAsync", ["query", "cancellation"]);
            })
            .InitVariableFromConstructorCall("result", _dtoName, ["items.ToList()", "items.GetPage()"])
            .ReturnVariable("result");


        methodBuilder.WithBody(methodBodyBuilder.Build());
        handlerClass.WithConstructor(constructor.Build());
        handlerClass.WithMethod(methodBuilder.Build());

        WriteFile(_handlerName, handlerClass.BuildAsString());
    }

    private void GenerateEndpoint()
    {
        var endpointClass = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.StaticKeyword,
                SyntaxKind.PartialKeyword
            ], _endpointClassName)
            .WithUsings([
                "Microsoft.AspNetCore.Mvc",
                "ITech.Cqrs.Cqrs.Queries",
                Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation
            ])
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature);

        var methodBuilder = new MethodBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.StaticKeyword,
                SyntaxKind.AsyncKeyword
            ], "Task<IResult>", Scheme.Configuration.Endpoint.FunctionName)
            .WithParameters([
                new ParameterOfMethodBuilder($"[AsParameters]{_queryName}", "query"),
                new ParameterOfMethodBuilder("IQueryDispatcher", "queryDispatcher"),
                new ParameterOfMethodBuilder("CancellationToken", "cancellation"),
            ])
            .WithProducesResponseTypeAttribute(_dtoName)
            .WithXmlDoc($"Get {Scheme.EntityScheme.EntityTitle.PluralTitle}",
                200,
                $"Returns {Scheme.EntityScheme.EntityTitle} list");

        var methodBodyBuilder = new MethodBodyBuilder()
            .InitVariableFromGenericAsyncMethodCall("result", "queryDispatcher", "DispatchAsync",
                [_queryName, _dtoName],
                ["query", "cancellation"])
            .ReturnTypedResultOk("result");

        methodBuilder.WithBody(methodBodyBuilder.Build());
        endpointClass.WithMethod(methodBuilder.Build());

        WriteFile(_endpointClassName, endpointClass.BuildAsString());

        EndpointMap = new EndpointMap(EntityScheme.EntityTitle.ToString(),
            Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature,
            "Get",
            Scheme.Configuration.Endpoint.Route,
            _endpointClassName,
            Scheme.Configuration.Endpoint.FunctionName);
    }
}