using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Teniry.CrudGenerator.Core.Generators.Core.SyntaxFactoryBuilders;

internal class ClassBuilder {
    private readonly List<MemberDeclarationSyntax> _fields = [];
    private readonly List<BaseTypeSyntax> _implementInterfaces = [];
    private readonly List<MemberDeclarationSyntax> _methods = [];
    private readonly List<PropertyBuilder> _propertyBuilders = [];
    private readonly List<string> _usings = [];
    private ClassDeclarationSyntax _classDeclaration;
    private FileScopedNamespaceDeclarationSyntax? _namespace;
    private SyntaxTriviaList _xmlDoc;

    public ClassBuilder(SyntaxKind[] modifiers, string className) {
        _classDeclaration = ClassDeclaration(className).AddModifiers(modifiers.Select(Token).ToArray());
    }

    public ClassBuilder WithNamespace(string @namespace) {
        _namespace = FileScopedNamespaceDeclaration(ParseName(@namespace));

        return this;
    }

    public ClassBuilder WithUsings(IEnumerable<string> usings) {
        _usings.AddRange(usings);

        return this;
    }

    public ClassBuilder Implements(string interfaceName, params string[] interfaceParams) {
        if (interfaceParams.Length > 0) {
            var arguments = new List<SyntaxNodeOrToken>();

            for (var i = 0; i < interfaceParams.Length; i++) {
                arguments.Add(IdentifierName(interfaceParams[i]));
                if (i != interfaceParams.Length - 1) {
                    arguments.Add(Token(SyntaxKind.CommaToken));
                }
            }

            _implementInterfaces.Add(
                SimpleBaseType(
                    GenericName(Identifier(interfaceName))
                        .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(arguments.ToArray())))
                )
            );

            return this;
        }

        _implementInterfaces.Add(SimpleBaseType(IdentifierName(interfaceName)));

        return this;
    }

    public ClassBuilder WithPrivateField(SyntaxKind[] modifiers, string fieldType, string fieldName) {
        var field = FieldDeclaration(
                VariableDeclaration(ParseTypeName(fieldType))
                    .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(fieldName))))
            )
            .WithModifiers(TokenList(modifiers.Select(Token).ToArray()));
        _fields.Add(field);

        return this;
    }

    public PropertyBuilder WithProperty(string typeName, string propertyName) {
        var propertyBuilder = new PropertyBuilder(typeName, propertyName);
        _propertyBuilders.Add(propertyBuilder);

        return propertyBuilder;
    }

    public ClassBuilder WithMethod(MethodDeclarationSyntax endpointMethod) {
        _methods.Add(endpointMethod);

        return this;
    }

    public ClassBuilder WithConstructor(ConstructorDeclarationSyntax constructorDeclaration) {
        _methods.Add(constructorDeclaration);

        return this;
    }

    public ClassBuilder WithXmlDoc(string summary, string returns = "", XmlDocException[]? exceptions = null) {
        var xmlDoc = new StringBuilder();
        xmlDoc.AppendLine(
            @$"
/// <summary>
///     {summary}
/// </summary>"
        );
        if (!string.IsNullOrEmpty(returns)) {
            xmlDoc.AppendLine($"/// <returns>{returns}</returns>");
        }

        if (exceptions is not null) {
            foreach (var exception in exceptions) {
                xmlDoc.AppendLine($"/// <exception cref=\"{exception.TypeName}\">{exception.Description}</exception>");
            }
        }

        _xmlDoc = ParseLeadingTrivia(xmlDoc.ToString());

        return this;
    }

    public CompilationUnitSyntax Build() {
        var compilationUnit = CompilationUnit();
        var usings = _usings.Select(x => UsingDirective(ParseName(x))).ToArray();
        compilationUnit = compilationUnit.AddUsings(usings);

        if (_implementInterfaces.Count > 0) {
            _classDeclaration = _classDeclaration.AddBaseListTypes(_implementInterfaces.ToArray());
        }

        _classDeclaration = _classDeclaration.AddMembers(_fields.ToArray());
        _classDeclaration =
            _classDeclaration.AddMembers(_propertyBuilders.Select(x => x.Build()).ToArray<MemberDeclarationSyntax>());
        _classDeclaration = _classDeclaration.AddMembers(_methods.ToArray());
        _classDeclaration = _classDeclaration.WithLeadingTrivia(_xmlDoc);
        _namespace = _namespace?.AddMembers(_classDeclaration);

        if (_namespace != null) {
            compilationUnit = compilationUnit.AddMembers(_namespace);
        }

        return compilationUnit;
    }

    public string BuildAsString() {
        // Normalize and get code as string
        var result = Build()
            .NormalizeWhitespace()
            .ToFullString();

        return result;
    }
}

internal class XmlDocException(string typeName, string description) {
    public string TypeName { get; set; } = typeName;
    public string Description { get; set; } = description;
}