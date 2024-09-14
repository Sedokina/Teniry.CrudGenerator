using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mars.Generators;

public class AttributeSyntaxReceiver<TAttribute> : ISyntaxReceiver
    where TAttribute : Attribute
{
    public IList<ClassDeclarationSyntax> Classes { get; } = new List<ClassDeclarationSyntax>();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
            classDeclarationSyntax.AttributeLists.Count > 0 &&
            classDeclarationSyntax.AttributeLists
                .Any(al => al.Attributes
                    .Any(a => a.Name.ToString().EnsureEndsWith("Attribute").Equals(typeof(TAttribute).Name))))
            Classes.Add(classDeclarationSyntax);
    }
}

public static class StringExtensions
{
    public static string EnsureEndsWith(
        this string source,
        string suffix)
    {
        if (source.EndsWith(suffix)) return source;
        return source + suffix;
    }
}