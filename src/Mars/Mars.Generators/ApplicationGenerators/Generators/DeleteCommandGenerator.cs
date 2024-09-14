using System.Collections.Generic;
using System.Text;
using Mars.Generators.ApplicationGenerators.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class DeleteCommandGenerator : BaseGenerator
{
    private readonly string _entityName;
    private readonly string _usingEntityNamespace;
    private readonly string _putIntoNamespace;
    private readonly string _commandName;
    private readonly string _handlerName;

    public DeleteCommandGenerator(GeneratorExecutionContext context, ISymbol symbol) : base(context, symbol)
    {
        _entityName = Symbol.Name;
        _usingEntityNamespace = Symbol.ContainingNamespace.ToString();
        _putIntoNamespace = Symbol.ContainingAssembly.Name;
        _commandName = Configuration.DeleteCommandCommandGenerator.GetCommandName(_entityName);
        _handlerName = Configuration.DeleteCommandCommandGenerator.GetHandlerName(_entityName);
    }

    public void RunGenerator()
    {
        GenerateCommand(Configuration.DeleteCommandCommandGenerator.CommandTemplatePath);
        GenerateHandler(Configuration.DeleteCommandCommandGenerator.HandlerTemplatePath);
    }

    private void GenerateCommand(string templatePath)
    {
        var template = ReadTemplate(templatePath);

        var propertiesOfClass = ((INamedTypeSymbol)Symbol).GetMembers().OfType<IPropertySymbol>();
        var result = "";
        foreach (var propertySymbol in propertiesOfClass)
        {
            // skip adding to command property if it is not id of the entity
            var propertyNameLower = propertySymbol.Name.ToLower();
            if (!propertyNameLower.Equals("id") && !propertyNameLower.Equals($"{Symbol.Name}id"))
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
            EntityNamespace = _usingEntityNamespace,
            PutIntoNamespace = _putIntoNamespace,
            Properties = result
        });

        Context.AddSource($"{_commandName}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }

    private void GenerateHandler(string templatePath)
    {
        var template = ReadTemplate(templatePath);

        var propertiesOfClass = ((INamedTypeSymbol)Symbol).GetMembers().OfType<IPropertySymbol>();
        var result = new List<string>();
        foreach (var propertySymbol in propertiesOfClass)
        {
            // skip adding to command property if it is not id of the entity
            var propertyNameLower = propertySymbol.Name.ToLower();
            if (!propertyNameLower.Equals("id") && !propertyNameLower.Equals($"{Symbol.Name}id"))
            {
                continue;
            }

            result.Add($"command.{propertySymbol.Name}");
        }

        var sourceCode = template.Render(new
        {
            EntityName = _entityName,
            EntityNamespace = _usingEntityNamespace,
            PutIntoNamespace = _putIntoNamespace,
            CommandName =  _commandName,
            FindProperties = string.Join(", ", result)
        });


        Context.AddSource($"{_handlerName}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }
}