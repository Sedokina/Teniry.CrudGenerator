using System.Text;
using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class GenerateCreateCommand : CrudGenerator
{
    private readonly GeneratorExecutionContext _context;
    private readonly ISymbol _symbol;
    private readonly string _entityName;
    private readonly string _usingEntityNamespace;
    private readonly string _putIntoNamespace;
    private readonly string _commandName;
    private readonly string _handlerName;

    public GenerateCreateCommand(GeneratorExecutionContext context, ISymbol symbol)
    {
        _context = context;
        _symbol = symbol;
        _entityName = _symbol.Name;
        _usingEntityNamespace = _symbol.ContainingNamespace.ToString();
        _putIntoNamespace = _symbol.ContainingAssembly.Name;
        _commandName = Configuration.CreateCommandCommandGenerator.GetCommandName(_entityName);
        _handlerName = Configuration.CreateCommandCommandGenerator.GetHandlerName(_entityName);
    }

    public void RunGenerator()
    {
        GenerateCommand(Configuration.CreateCommandCommandGenerator.CommandTemplatePath);
        GenerateHandler(Configuration.CreateCommandCommandGenerator.HandlerTemplatePath);
    }

    private void GenerateCommand(string templatePath)
    {
        var template = ReadTemplate(templatePath);

        var propertiesOfClass = ((INamedTypeSymbol)_symbol).GetMembers().OfType<IPropertySymbol>();
        var result = "";
        foreach (var propertySymbol in propertiesOfClass)
        {
            // skip adding to command id of the entity
            var propertyNameLower = propertySymbol.Name.ToLower();
            if (propertyNameLower.Equals("id") || propertyNameLower.Equals($"{_symbol.Name}id"))
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
            EntityName = _entityName,
            Namespace = _usingEntityNamespace,
            PutIntoNamespace = _putIntoNamespace,
            Properties = result
        });

        _context.AddSource($"{_commandName}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }

    private void GenerateHandler(string templatePath)
    {
        var template = ReadTemplate(templatePath);

        var sourceCode = template.Render(new
        {
            EntityName = _entityName,
            Namespace = _usingEntityNamespace,
            PutIntoNamespace = _putIntoNamespace,
            CommandName = _commandName,
        });

        _context.AddSource($"{_handlerName}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }
}