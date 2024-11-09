using System;
using System.Linq;
using System.Threading;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.Formatters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

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
//
//     private void GenerateEndpoint(string templatePath)
//     {
//         var endpointNamespace = Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature;
//         var businessLogicNamespace =
//             Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation;
//         var compilationUnit = SyntaxFactory.CompilationUnit();
//         compilationUnit = compilationUnit.AddUsings([
//             SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.AspNetCore.Mvc")),
//             SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("ITech.Cqrs.Cqrs.Queries")),
//             SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(businessLogicNamespace))
//         ]);
//
//         var @namespace = SyntaxFactory
//             .FileScopedNamespaceDeclaration(SyntaxFactory.ParseName(endpointNamespace));
//
//         //  Create a class: (class Order)
//         var classDeclaration = SyntaxFactory.ClassDeclaration(_endpointClassName);
//
//         // Add the public modifier: (public class Order)
//         classDeclaration = classDeclaration.AddModifiers([
//             SyntaxFactory.Token(SyntaxKind.PublicKeyword),
//             SyntaxFactory.Token(SyntaxKind.StaticKeyword),
//             SyntaxFactory.Token(SyntaxKind.PartialKeyword)
//         ]);
//
//         // parameters for method to accept
//         var routeParameters = EntityScheme.PrimaryKeys
//             .Select(x => SyntaxFactory
//                 .Parameter(SyntaxFactory.Identifier(x.PropertyNameAsMethodParameterName))
//                 .WithType(SyntaxFactory.ParseTypeName(x.TypeName)))
//             .ToArray();
//
//         // Create query object
//         ExpressionSyntax initializationExpression = SyntaxFactory.ObjectCreationExpression(
//             SyntaxFactory.Token(SyntaxKind.NewKeyword),
//             SyntaxFactory.ParseTypeName(_queryName),
//             SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
//                 routeParameters.Select(x => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(x.Identifier.Text)))
//                     .ToArray()
//             )),
//             null
//         );
//
//         // Initialize query variable with query object value
//         var variableDeclarator = SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("query"), null,
//             SyntaxFactory.EqualsValueClause(initializationExpression));
//         var variableDeclaration = SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("var"))
//             .WithVariables(SyntaxFactory.SeparatedList<VariableDeclaratorSyntax>().Add(variableDeclarator));
//
//         var localDeclaration = SyntaxFactory.LocalDeclarationStatement(variableDeclaration);
//
//         // call query dispatcher
//         var a = SyntaxFactory.AwaitExpression(
//             SyntaxFactory.InvocationExpression(
//                 SyntaxFactory.MemberAccessExpression(
//                     SyntaxKind.SimpleMemberAccessExpression,
//                     SyntaxFactory.IdentifierName("queryDispatcher"),
//                     SyntaxFactory.GenericName(SyntaxFactory.Identifier("DispatchAsync"))
//                         .WithTypeArgumentList(
//                             SyntaxFactory.TypeArgumentList(
//                                 SyntaxFactory.SeparatedList<TypeSyntax>(
//                                     new SyntaxNodeOrToken[]
//                                     {
//                                         SyntaxFactory.IdentifierName(_queryName),
//                                         SyntaxFactory.Token(SyntaxKind.CommaToken),
//                                         SyntaxFactory.IdentifierName(_dtoName)
//                                     }
//                                 )
//                             )
//                         )
//                 ),
//                 SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList([
//                     SyntaxFactory.Argument(SyntaxFactory.IdentifierName("query")),
//                     SyntaxFactory.Argument(SyntaxFactory.IdentifierName("cancellation")),
//                 ]))
//             )
//         );
//
//         var variableDeclaratorResultVariable = SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("result"),
//             null,
//             SyntaxFactory.EqualsValueClause(a));
//         var variableDeclarationResultVariable = SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("var"))
//             .WithVariables(
//                 SyntaxFactory.SeparatedList<VariableDeclaratorSyntax>().Add(variableDeclaratorResultVariable));
//
//         var localDeclarationResultVariable = SyntaxFactory.LocalDeclarationStatement(variableDeclarationResultVariable);
//
//
//         // Return result
//         var returnStatement = SyntaxFactory.ReturnStatement(
//             SyntaxFactory.InvocationExpression(
//                 SyntaxFactory.MemberAccessExpression(
//                     SyntaxKind.SimpleMemberAccessExpression,
//                     SyntaxFactory.IdentifierName("TypedResults"),
//                     SyntaxFactory.IdentifierName("Ok")),
//                 SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList([
//                     SyntaxFactory.Argument(SyntaxFactory.IdentifierName("result"))
//                 ]))));
//
//
//         var doc = @$"
// /// <summary>
// ///     Get {Scheme.EntityScheme.EntityTitle} by id
// /// </summary>
// /// <response code=""200"">Returns full {Scheme.EntityScheme.EntityTitle} data</response>
// ";
//         // Create a method
//         var methodDeclaration = SyntaxFactory.MethodDeclaration(
//                 SyntaxFactory.ParseTypeName("Task<IResult>"),
//                 Scheme.Configuration.Endpoint.FunctionName)
//             .AddParameterListParameters(routeParameters)
//             .AddParameterListParameters([
//                 SyntaxFactory
//                     .Parameter(SyntaxFactory.Identifier("queryDispatcher"))
//                     .WithType(SyntaxFactory.ParseTypeName("IQueryDispatcher")),
//                 SyntaxFactory
//                     .Parameter(SyntaxFactory.Identifier("cancellation"))
//                     .WithType(SyntaxFactory.ParseTypeName(nameof(CancellationToken)))
//             ])
//             .AddModifiers([
//                 SyntaxFactory.Token(SyntaxKind.PublicKeyword),
//                 SyntaxFactory.Token(SyntaxKind.StaticKeyword),
//                 SyntaxFactory.Token(SyntaxKind.AsyncKeyword),
//             ])
//             .WithAttributeLists(
//                 SyntaxFactory.SingletonList(
//                     SyntaxFactory.AttributeList(
//                         SyntaxFactory.SingletonSeparatedList(
//                             SyntaxFactory.Attribute(
//                                     SyntaxFactory.IdentifierName("ProducesResponseType"))
//                                 .WithArgumentList(
//                                     SyntaxFactory.AttributeArgumentList(
//                                         SyntaxFactory.SeparatedList<AttributeArgumentSyntax>(
//                                             new SyntaxNodeOrToken[]
//                                             {
//                                                 SyntaxFactory.AttributeArgument(
//                                                     SyntaxFactory.TypeOfExpression(
//                                                         SyntaxFactory.IdentifierName(_dtoName))),
//                                                 SyntaxFactory.Token(SyntaxKind.CommaToken),
//                                                 SyntaxFactory.AttributeArgument(
//                                                     SyntaxFactory.LiteralExpression(
//                                                         SyntaxKind.NumericLiteralExpression,
//                                                         SyntaxFactory.Literal(200)))
//                                             }))))).WithLeadingTrivia(SyntaxFactory.ParseLeadingTrivia(doc))
//                 ))
//             .WithBody(SyntaxFactory.Block(new List<StatementSyntax>()
//             {
//                 localDeclaration,
//                 localDeclarationResultVariable,
//                 returnStatement,
//             }));
//         // SyntaxFactory.ParseLeadingTrivia(doc);
//
//         // Add the field, the property and method to the class.
//         classDeclaration = classDeclaration.AddMembers(methodDeclaration);
//
//         // Add the class to the namespace.
//         @namespace = @namespace.AddMembers(classDeclaration);
//
//         compilationUnit = compilationUnit.AddMembers(@namespace);
//
//         // Normalize and get code as string.
//         var code = compilationUnit
//             .NormalizeWhitespace()
//             .ToFullString();
//
//         Context.AddSource($"{_endpointClassName}.g.cs", code);
//     }

    public void GenerateEndpoint(string a)
    {
        var endpointClass = new ClassBuilder(_endpointClassName)
            .WithUsings([
                "Microsoft.AspNetCore.Mvc",
                "ITech.Cqrs.Cqrs.Queries",
                Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation
            ])
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature);

        var methodXmlDoc = @$"
/// <summary>
///     Get {Scheme.EntityScheme.EntityTitle} by id
/// </summary>
/// <response code=""200"">Returns full {Scheme.EntityScheme.EntityTitle} data</response>
";
        var methodBuilder = new MethodBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.StaticKeyword,
                SyntaxKind.AsyncKeyword
            ], "Task<IResult>", Scheme.Configuration.Endpoint.FunctionName)
            .WithParameters(EntityScheme.PrimaryKeys
                .Select(x => new ParameterOfMethodBuilder(x.TypeName, x.PropertyNameAsMethodParameterName))
                .Append(new ParameterOfMethodBuilder("IQueryDispatcher", "queryDispatcher"))
                .Append(new ParameterOfMethodBuilder("CancellationToken", "cancellation"))
                .ToList())
            .WithXmlDoc(methodXmlDoc)
            .WithProducesResponseTypeAttribute(_dtoName);

        var methodBodyBuilder = new MethodBodyBuilder()
            .InitVariableFromConstructorCall("query", _queryName, EntityScheme.PrimaryKeys)
            .InitVariableFromGenericMethodCall("result", "queryDispatcher", "DispatchAsync", [_queryName, _dtoName],
                ["query", "cancellation"])
            .ReturnTypedResultOk("result");

        methodBuilder.WithBody(methodBodyBuilder.Build());
        endpointClass.WithMethod(methodBuilder.Build());

        Context.AddSource($"{_endpointClassName}.g.cs", endpointClass.BuildAsString());

        EndpointMap = new EndpointMap(EntityScheme.EntityName.ToString(),
            Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature,
            "Get",
            Scheme.Configuration.Endpoint.Route,
            $"{_endpointClassName}.{Scheme.Configuration.Endpoint.FunctionName}");
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
