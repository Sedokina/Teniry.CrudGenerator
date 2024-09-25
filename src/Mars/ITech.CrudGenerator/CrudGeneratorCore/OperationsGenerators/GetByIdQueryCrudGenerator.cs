using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.Formatters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators;

internal class
    GetByIdQueryCrudGenerator : BaseOperationCrudGenerator<CqrsOperationWithReturnValueGeneratorConfiguration>
{
    private readonly string _dtoName;
    private readonly string _handlerName;
    private readonly string _queryName;
    private readonly string _endpointClassName;

    public GetByIdQueryCrudGenerator(
        GeneratorExecutionContext context,
        CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration> scheme) : base(context, scheme)
    {
        _queryName = Scheme.Configuration.Operation.Name;
        _handlerName = Scheme.Configuration.Handler.Name;
        _dtoName = Scheme.Configuration.Dto.Name;
        _endpointClassName = Scheme.Configuration.Endpoint.Name;
    }

    public override void RunGenerator()
    {
        GenerateQuery(Scheme.Configuration.Operation.TemplatePath);
        GenerateHandler(Scheme.Configuration.Handler.TemplatePath);
        GenerateDto(Scheme.Configuration.Dto.TemplatePath);
        if (Scheme.Configuration.Endpoint.Generate)
        {
            GenerateEndpoint(Scheme.Configuration.Endpoint.TemplatePath);
        }
    }

    private void GenerateQuery(string templatePath)
    {
        var properties = EntityScheme.PrimaryKeys.FormatAsProperties();
        var constructorParameters = EntityScheme.PrimaryKeys.FormatAsMethodDeclarationParameters();
        var constructorBody = EntityScheme.PrimaryKeys.FormatAsConstructorBody();
        var model = new
        {
            QueryName = _queryName,
            DtoName = _dtoName,
            Properties = properties,
            ConstructorParameters = constructorParameters,
            ConstructorBody = constructorBody
        };

        WriteFile(templatePath, model, _queryName);
    }

    private void GenerateDto(string templatePath)
    {
        var properties = EntityScheme.Properties.FormatAsProperties();
        var model = new
        {
            DtoName = _dtoName,
            Properties = properties
        };

        WriteFile(templatePath, model, _dtoName);
    }

    private void GenerateHandler(string templatePath)
    {
        var findParameters = EntityScheme.PrimaryKeys.FormatAsMethodCallParameters("query");
        var model = new
        {
            QueryName = _queryName,
            HandlerName = _handlerName,
            DtoName = _dtoName,
            FindParameters = findParameters
        };

        WriteFile(templatePath, model, _handlerName);
    }

    private void GenerateEndpoint(string templatePath)
    {
        var endpointNamespace = Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature;
        var businessLogicNamespace =
            Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation;
        var compilationUnit = SyntaxFactory.CompilationUnit();
        compilationUnit = compilationUnit.AddUsings([
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.AspNetCore.Mvc")),
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("ITech.Cqrs.Cqrs.Queries")),
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(businessLogicNamespace))
        ]);

        var @namespace = SyntaxFactory
            .FileScopedNamespaceDeclaration(SyntaxFactory.ParseName(endpointNamespace));

        //  Create a class: (class Order)
        var classDeclaration = SyntaxFactory.ClassDeclaration(_endpointClassName);

        // Add the public modifier: (public class Order)
        classDeclaration = classDeclaration.AddModifiers([
            SyntaxFactory.Token(SyntaxKind.PublicKeyword),
            SyntaxFactory.Token(SyntaxKind.StaticKeyword),
            SyntaxFactory.Token(SyntaxKind.PartialKeyword)
        ]);

        // Create a stament with the body of a method.
        var syntax = SyntaxFactory.ParseStatement("return TypedResults.Ok();");
        // var assignment = SyntaxFactory.AssignmentExpression(SyntaxKind.VariableDeclaration, SyntaxFactory.Parse)

        // SyntaxFactory.LocalDeclarationStatement();
        // SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName("query"),
        //     SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName("_queryName")));
        // Create a method
        var routeParameters = EntityScheme.PrimaryKeys
            .Select(x => SyntaxFactory
                .Parameter(SyntaxFactory.Identifier(x.PropertyNameAsMethodParameterName))
                .WithType(SyntaxFactory.ParseTypeName(x.TypeName)))
            .ToArray();

        var methodDeclaration = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.ParseTypeName("Task<IResult>"),
                Scheme.Configuration.Endpoint.FunctionName)
            .AddParameterListParameters(routeParameters)
            .AddParameterListParameters([
                SyntaxFactory
                    .Parameter(SyntaxFactory.Identifier("queryDispatcher"))
                    .WithType(SyntaxFactory.ParseTypeName("IQueryDispatcher")),
                SyntaxFactory
                    .Parameter(SyntaxFactory.Identifier("cancellation"))
                    .WithType(SyntaxFactory.ParseTypeName(nameof(CancellationToken)))
            ])
            .AddModifiers([
                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                SyntaxFactory.Token(SyntaxKind.AsyncKeyword),
            ])
            .WithBody(SyntaxFactory.Block(syntax));

        // Add the field, the property and method to the class.
        classDeclaration = classDeclaration.AddMembers(methodDeclaration);

        // Add the class to the namespace.
        @namespace = @namespace.AddMembers(classDeclaration);

        compilationUnit = compilationUnit.AddMembers(@namespace);

        // Normalize and get code as string.
        var code = compilationUnit
            .NormalizeWhitespace()
            .ToFullString();

        Context.AddSource($"{_endpointClassName}.g.cs", code);
    }

    // private void GenerateEndpoint(string templatePath)
    // {
    //     var routeParams = EntityScheme.PrimaryKeys.FormatAsMethodDeclarationParameters();
    //     var constructorParameters = EntityScheme.PrimaryKeys.FormatAsMethodCallArguments();
    //     var model = new
    //     {
    //         EndpointClassName = _endpointClassName,
    //         FunctionName = Scheme.Configuration.Endpoint.FunctionName,
    //         QueryName = _queryName,
    //         DtoName = _dtoName,
    //         RouteParams = routeParams,
    //         QueryConstructorParameters = constructorParameters
    //     };
    //
    //     WriteFile(templatePath, model, _endpointClassName);
    //
    //     EndpointMap = new EndpointMap(EntityScheme.EntityName.ToString(),
    //         Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature,
    //         "Get",
    //         Scheme.Configuration.Endpoint.Route,
    //         $"{_endpointClassName}.{Scheme.Configuration.Endpoint.FunctionName}");
    // }
}