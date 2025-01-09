using System.Collections.Generic;
using System.Linq;
using ITech.CrudGenerator.Core.Configurations.Global;
using ITech.CrudGenerator.Core.Generators.Core;
using ITech.CrudGenerator.Core.Generators.Core.SyntaxFactoryBuilders;
using ITech.CrudGenerator.Core.Generators.Core.SyntaxFactoryBuilders.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ITech.CrudGenerator;

internal class EndpointsMapGenerator : BaseGenerator
{
    private readonly List<EndpointMap> _endpointsMaps;
    private readonly string _endpointMapsClassName;

    public EndpointsMapGenerator(
        List<EndpointMap> endpointsMaps,
        GlobalCrudGeneratorConfiguration globalConfiguration)
        : base(globalConfiguration.AutogeneratedFileText,
            globalConfiguration.NullableEnable)
    {
        _endpointsMaps = endpointsMaps;
        _endpointMapsClassName = "GeneratedEndpointsMapExtension";
    }

    public override void RunGenerator()
    {
        var endpointsMapClass =
            new ClassBuilder([SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword], _endpointMapsClassName)
                .WithNamespace("AutogeneratedEndpoints")
                .WithUsings(_endpointsMaps.Select(x => x.EndpointNamespace).Distinct().ToArray());

        var mapMethod = new MethodBuilder([SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword], "void",
                "MapGeneratedEndpoints")
            .WithParameters([new ParameterOfMethodBuilder([SyntaxKind.ThisKeyword], "WebApplication", "app")]);

        var mapBody = Block();

        StatementSyntax[] mapStatements = _endpointsMaps.Select(endpointMap => ExpressionStatement(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("app"),
                                        IdentifierName($"Map{endpointMap.HttpMethod}")))
                                .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList<ArgumentSyntax>(
                                            new SyntaxNodeOrToken[]
                                            {
                                                Argument(
                                                    LiteralExpression(
                                                        SyntaxKind.StringLiteralExpression,
                                                        Literal(endpointMap.EndpointRoute))),
                                                Token(SyntaxKind.CommaToken),
                                                Argument(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName(endpointMap.ClassName),
                                                        IdentifierName(endpointMap.FunctionName)))
                                            }))),
                            IdentifierName("WithTags")))
                    .WithArgumentList(
                        ArgumentList(
                            SingletonSeparatedList(
                                Argument(
                                    LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        Literal(endpointMap.EntityTitle))))))))
            .ToArray<StatementSyntax>();
        mapBody = mapBody.AddStatements(mapStatements);

        mapMethod.WithBody(mapBody);
        endpointsMapClass.WithMethod(mapMethod.Build());
        WriteFile(_endpointMapsClassName, endpointsMapClass.BuildAsString());
    }
}