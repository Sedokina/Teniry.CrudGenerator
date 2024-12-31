using System.Threading;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.Formatters;
using Microsoft.CodeAnalysis.CSharp;

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
        _queryName = Scheme.Configuration.Operation.Name;
        _listItemDtoName = Scheme.Configuration.DtoListItem.Name;
        _dtoName = Scheme.Configuration.Dto.Name;
        _filterName = Scheme.Configuration.Filter.Name;
        _handlerName = Scheme.Configuration.Handler.Name;
        _endpointClassName = Scheme.Configuration.Endpoint.Name;
    }

    public override void RunGenerator()
    {
        GenerateQuery(Scheme.Configuration.Operation.TemplatePath);
        GenerateListItemDto();
        GenerateDto(Scheme.Configuration.Dto.TemplatePath);
        GenerateFilter(Scheme.Configuration.Filter.TemplatePath);
        GenerateHandler();
        if (Scheme.Configuration.Endpoint.Generate)
        {
            GenerateEndpoint();
        }
    }

    private void GenerateQuery(string templatePath)
    {
        var properties = EntityScheme.Properties.FormatAsFilterProperties();
        var sortKeys = EntityScheme.SortableProperties.FormatAsSortKeys();

        var model = new
        {
            QueryName = _queryName,
            DtoName = _dtoName,
            PutIntoNamespace = Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation,
            Properties = properties,
            SortKeys = sortKeys
        };
        WriteFile(templatePath, model, _queryName);
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
            dtoClass.WithProperty(property.TypeName, property.PropertyName, property.DefaultValue);
        }

        WriteFile(_listItemDtoName, dtoClass.BuildAsString());
    }

    private void GenerateDto(string templatePath)
    {
        var model = new
        {
            DtoName = _dtoName,
            ListItemDtoName = _listItemDtoName
        };

        WriteFile(templatePath, model, _dtoName);
    }

    private void GenerateFilter(string templatePath)
    {
        var properties = EntityScheme.Properties.FormatAsFilterProperties();
        var filter = EntityScheme.Properties.FormatAsFilterBody();
        var sorts = EntityScheme.SortableProperties.FormatAsSortCalls();
        var defaultSort = FormatDefaultSort(EntityScheme.DefaultSort);

        var model = new
        {
            FilterName = _filterName,
            Properties = properties,
            Filter = filter,
            Sorts = sorts,
            DefaultSort = defaultSort
        };
        WriteFile(templatePath, model, _filterName);
    }

    private static string FormatDefaultSort(EntityDefaultSort? defaultSort)
    {
        if (defaultSort != null)
        {
            return defaultSort.Direction.Equals("asc")
                ? $"query.OrderBy(x => x.{defaultSort.PropertyName});"
                : $"query.OrderByDescending(x => x.{defaultSort.PropertyName});";
        }

        return "base.DefaultSort(query);";
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

        var constructor = new MethodBuilder([SyntaxKind.PublicKeyword], "", _handlerName)
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
        handlerClass.WithMethod(constructor.Build());
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

        EndpointMap = new EndpointMap(EntityScheme.EntityName.ToString(),
            Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature,
            "Get",
            Scheme.Configuration.Endpoint.Route,
            $"{_endpointClassName}.{Scheme.Configuration.Endpoint.FunctionName}");
    }
}