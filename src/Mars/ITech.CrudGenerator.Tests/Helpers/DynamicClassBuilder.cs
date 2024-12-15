using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ITech.CrudGenerator.Tests.Helpers;

public static class DynamicClassBuilder
{
    public static ISymbol GenerateEntity(string entityName, string body = "")
    {
        var entityClass = $@"using System;

namespace ITech.CrudGenerator.Tests {{
    public class {entityName}
    {{
        {body}
    }}
}}
";

        var syntaxTree = CSharpSyntaxTree.ParseText(entityClass);
        var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        var compilation = CSharpCompilation.Create(
            Assembly.GetExecutingAssembly().FullName,
            [syntaxTree],
            references: new[] { mscorlib },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var symbol = compilation.GetSymbolsWithName(entityName).First();
        return symbol;
    }
}