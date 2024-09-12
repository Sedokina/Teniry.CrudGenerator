using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace Mars.Generators;

[Generator]
public class CreateCommandGenerator : ISourceGenerator
{
    private const string CommandResourcePath = "Mars.Generators.Templates.CreateCommand.txt";
    private const string HandlerResourcePath = "Mars.Generators.Templates.CreateHandler.txt";

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver<GenerateCreateCommandAttribute>());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not AttributeSyntaxReceiver<GenerateCreateCommandAttribute> syntaxReceiver)
        {
            return;
        }

        foreach (var classSyntax in syntaxReceiver.Classes)
        {
            // Converting the class to semantic model to access much more meaningful data.
            var model = context.Compilation.GetSemanticModel(classSyntax.SyntaxTree);
            // Parse to declared symbol, so you can access each part of code separately, such as interfaces, methods, members, contructor parameters etc.
            var symbol = model.GetDeclaredSymbol(classSyntax) ?? throw new ArgumentException("symbol");

            GenerateCommand(context, symbol);
            GenerateHandler(context, symbol);
        }
    }

    private void GenerateCommand(GeneratorExecutionContext context, ISymbol symbol)
    {
        // Generate the real source code. Pass the template parameter if there is a overriden template.
        var template = Template.Parse(GetEmbeddedResource(CommandResourcePath));

        var propertiesOfClass = ((INamedTypeSymbol)symbol).GetMembers().OfType<IPropertySymbol>();
        var result = "";
        foreach (var propertySymbol in propertiesOfClass)
        {
            // skip adding to command id of the entity
            var propertyNameLower = propertySymbol.Name.ToLower();
            if (propertyNameLower.Equals("id") || propertyNameLower.Equals($"{symbol.Name}id"))
            {
                continue;
            }

            // skip adding to command if not primitive type
            if (!propertySymbol.Type.IsSimple())
            {
                continue;
            }

            // For DateTimeOffset and other date variations remove system from the property type declaration
            var propertyTypeName = propertySymbol.Type.ToString().ToLower().StartsWith("system.")
                ? propertySymbol.Type.MetadataName
                : propertySymbol.Type.ToString();

            result += $"public {propertyTypeName} {propertySymbol.Name} {{ get; set; }}\n\t";
        }

        result = result.TrimEnd();

        var sourceCode = template.Render(new
        {
            ClassName = symbol.Name,
            Namespace = symbol.ContainingNamespace,
            PreferredNamespace = symbol.ContainingAssembly.Name,
            Properties = result
        });

        context.AddSource(
            $"Create{symbol.Name}Command.g.cs",
            SourceText.From(sourceCode, Encoding.UTF8));
    }

    private void GenerateHandler(GeneratorExecutionContext context, ISymbol symbol)
    {
        // Generate the real source code. Pass the template parameter if there is a overriden template.
        var template = Template.Parse(GetEmbeddedResource(HandlerResourcePath));

        var sourceCode = template.Render(new
        {
            ClassName = symbol.Name,
            Namespace = symbol.ContainingNamespace,
            PreferredNamespace = symbol.ContainingAssembly.Name,
            CommandName = $"Create{symbol.Name}Command",
            EntityName = symbol.Name
        });

        context.AddSource(
            $"Create{symbol.Name}Handler.g.cs",
            SourceText.From(sourceCode, Encoding.UTF8));
    }


    private string GetEmbeddedResource(string path)
    {
        using var stream = GetType().Assembly.GetManifestResourceStream(path);

        using var streamReader = new StreamReader(stream ?? throw new InvalidOperationException());

        return streamReader.ReadToEnd();
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class GenerateCreateCommandAttribute : Attribute
{
}

public static class TypeExtensions
{
    /// <summary>
    ///     Returns true if type is primitive, nullable primitive or has type converter from and to primitive type
    /// </summary>
    /// <param name="type">Type to check</param>
    /// <param name="compilation"></param>
    /// <returns>
    ///     This returns true for:<br/>
    ///         - All primitive types<br/>
    ///         - All enums<br/>
    ///         - strings<br/>
    ///         - decimals<br/>
    ///         - DateTime<br/>
    ///         - DateTimeOffset<br/>
    ///         - TimeSpan<br/>
    ///         - Uri<br/>
    ///         - Guid<br/>
    ///         - Nullable of any of the types above<br/>
    ///         - numerous other types that have native TypeConverters implemented (see here on the Derived section)<br/>
    /// 
    /// This approach works well since most frameworks support TypeConverters natively,
    /// like XML and Json serialization libraries, and you can then use the same converter to parse the values while reading.
    /// </returns>
    /// <remarks>
    ///     From: https://stackoverflow.com/a/65079923/10837606
    /// </remarks>
    public static bool IsSimple(this ITypeSymbol type)
    {
        switch (type.SpecialType)
        {
            case SpecialType.System_Boolean:
            case SpecialType.System_SByte:
            case SpecialType.System_Int16:
            case SpecialType.System_Int32:
            case SpecialType.System_Int64:
            case SpecialType.System_Byte:
            case SpecialType.System_UInt16:
            case SpecialType.System_UInt32:
            case SpecialType.System_UInt64:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_Char:
            case SpecialType.System_String:
            case SpecialType.System_DateTime:
            case SpecialType.System_Decimal:
                return true;
            default:
                if (type.NullableAnnotation == NullableAnnotation.Annotated)
                {
                    return IsSimple(((INamedTypeSymbol)type).TypeArguments[0]);
                }

                if (type.IsValueType && type.IsSealed && type.IsUnmanagedType)
                {
                    return true;
                }

                return false;
        }
    }
}