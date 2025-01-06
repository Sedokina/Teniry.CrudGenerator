using System.Collections.Generic;
using System.Linq;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core.SyntaxFactoryBuilders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator;

internal class EndpointsMapGenerator : BaseGenerator
{
    private readonly List<EndpointMap> _endpointsMaps;
    private readonly string _endpointMapsClassName;

    public EndpointsMapGenerator(
        List<EndpointMap> endpointsMaps,
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration)
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
            .WithParameters([new ParameterOfMethodBuilder("WebApplication", "app", [SyntaxKind.ThisKeyword])]);

        var mapBody = SyntaxFactory.Block();

        StatementSyntax[] mapStatements = _endpointsMaps.Select(endpointMap => SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName("app"),
                                        SyntaxFactory.IdentifierName($"Map{endpointMap.HttpMethod}")))
                                .WithArgumentList(
                                    SyntaxFactory.ArgumentList(
                                        SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                            new SyntaxNodeOrToken[]
                                            {
                                                SyntaxFactory.Argument(
                                                    SyntaxFactory.LiteralExpression(
                                                        SyntaxKind.StringLiteralExpression,
                                                        SyntaxFactory.Literal(endpointMap.EndpointRoute))),
                                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                SyntaxFactory.Argument(
                                                    SyntaxFactory.MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        SyntaxFactory.IdentifierName(endpointMap.ClassName),
                                                        SyntaxFactory.IdentifierName(endpointMap.FunctionName)))
                                            }))),
                            SyntaxFactory.IdentifierName("WithTags")))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal(endpointMap.EntityTitle))))))))
            .ToArray<StatementSyntax>();
        mapBody = mapBody.AddStatements(mapStatements);

        mapMethod.WithBody(mapBody);
        endpointsMapClass.WithMethod(mapMethod.Build());
        WriteFile(_endpointMapsClassName, endpointsMapClass.BuildAsString());
    }
}