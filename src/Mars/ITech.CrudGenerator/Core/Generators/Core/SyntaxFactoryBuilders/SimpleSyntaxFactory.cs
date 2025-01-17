using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ITech.CrudGenerator.Core.Generators.Core.SyntaxFactoryBuilders;

public static class SimpleSyntaxFactory {
    public static ObjectCreationExpressionSyntax CallConstructor(
        string className,
        List<ExpressionSyntax> constructorArguments
    ) {
        return ObjectCreationExpression(
            Token(SyntaxKind.NewKeyword),
            ParseTypeName(className),
            ArgumentList(SeparatedList(constructorArguments.Select(Argument).ToArray())),
            null
        );
    }

    public static InvocationExpressionSyntax CallMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<ExpressionSyntax> arguments
    ) {
        return InvocationExpression(
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(objectWithMethod),
                IdentifierName(Identifier(methodNameToCall))
            ),
            ArgumentList(SeparatedList(arguments.Select(Argument).ToArray()))
        );
    }

    public static AwaitExpressionSyntax CallAsyncMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<ExpressionSyntax> arguments
    ) {
        return AwaitExpression(CallMethod(objectWithMethod, methodNameToCall, arguments));
    }

    public static InvocationExpressionSyntax CallGenericMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodGenericTypeNames,
        List<ExpressionSyntax> arguments
    ) {
        return InvocationExpression(
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(objectWithMethod),
                GenericName(Identifier(methodNameToCall))
                    .WithTypeArgumentList(
                        TypeArgumentList(SeparatedList<TypeSyntax>(methodGenericTypeNames.Select(IdentifierName)))
                    )
            ),
            ArgumentList(SeparatedList(arguments.Select(Argument).ToArray()))
        );
    }

    public static AwaitExpressionSyntax CallGenericAsyncMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodGenericTypeNames,
        List<ExpressionSyntax> arguments
    ) {
        return AwaitExpression(
            CallGenericMethod(
                objectWithMethod,
                methodNameToCall,
                methodGenericTypeNames,
                arguments
            )
        );
    }

    public static IdentifierNameSyntax Variable(string variableName) {
        return IdentifierName(variableName);
    }

    public static MemberAccessExpressionSyntax Property(string objectName, string propertyName) {
        return MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(objectName),
            IdentifierName(propertyName)
        );
    }

    public static SimpleLambdaExpressionSyntax Expression(string variableName, bool suppressNullableWarning = false) {
        var memberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName("x"),
            IdentifierName(variableName)
        );

        ExpressionSyntax expressionBody = suppressNullableWarning ?
            PostfixUnaryExpression(SyntaxKind.SuppressNullableWarningExpression, memberAccess) :
            memberAccess;

        return SimpleLambdaExpression(Parameter(Identifier("x")))
            .WithExpressionBody(expressionBody);
    }

    public static InterpolatedStringExpressionSyntax InterpolatedString(string interpolatedString) {
        return InterpolatedStringExpression(Token(SyntaxKind.InterpolatedStringStartToken))
            .WithContents(
                SingletonList<InterpolatedStringContentSyntax>(
                    InterpolatedStringText().WithTextToken(
                        Token(
                            TriviaList(),
                            SyntaxKind.InterpolatedStringTextToken,
                            interpolatedString,
                            interpolatedString,
                            TriviaList()
                        )
                    )
                )
            );
    }

    public static ArrayCreationExpressionSyntax NewArray(string typeName, IEnumerable<string> parameters) {
        var arrayType = ArrayType(ParseTypeName(typeName))
            .WithRankSpecifiers(
                SingletonList(
                    ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression()))
                )
            );

        return ArrayCreationExpression(arrayType)
            .WithInitializer(
                InitializerExpression(
                    SyntaxKind.ArrayInitializerExpression,
                    SeparatedList<ExpressionSyntax>(parameters.Select(IdentifierName).ToArray())
                )
            );
    }

    public static ArrayCreationExpressionSyntax NewStringLiteralArray(IEnumerable<string> parameters) {
        var arrayType = ArrayType(ParseTypeName("string"))
            .WithRankSpecifiers(
                SingletonList(
                    ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression()))
                )
            );

        return ArrayCreationExpression(arrayType)
            .WithInitializer(
                InitializerExpression(
                    SyntaxKind.ArrayInitializerExpression,
                    SeparatedList<ExpressionSyntax>(
                        parameters.Select(p => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(p)))
                            .ToArray()
                    )
                )
            );
    }
}