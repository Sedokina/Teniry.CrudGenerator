using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.Extensions;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class CreateCommandGenerator : BaseGenerator
{
    private readonly string _entityName;
    private readonly string _usingEntityNamespace;
    private readonly string _putIntoNamespace;
    private readonly string _commandName;
    private readonly string _handlerName;

    public CreateCommandGenerator(GeneratorExecutionContext context, ISymbol symbol) : base(context, symbol)
    {
        _entityName = Symbol.Name;
        _usingEntityNamespace = Symbol.ContainingNamespace.ToString();
        _putIntoNamespace = Symbol.ContainingAssembly.Name;
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
        var propertiesOfClass = ((INamedTypeSymbol)Symbol).GetMembers().OfType<IPropertySymbol>();
        var result = "";
        foreach (var propertySymbol in propertiesOfClass)
        {
            // skip adding to command id of the entity
            var propertyNameLower = propertySymbol.Name.ToLower();
            if (propertyNameLower.Equals("id") || propertyNameLower.Equals($"{Symbol.Name}id"))
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

        var model = new
        {
            CommandName = _commandName,
            PutIntoNamespace = _putIntoNamespace,
            Properties = result
        };
        WriteFile(templatePath, model, _commandName);
    }

    private void GenerateHandler(string templatePath)
    {
        var model = new
        {
            EntityName = _entityName,
            EntityNamespace = _usingEntityNamespace,
            PutIntoNamespace = _putIntoNamespace,
            CommandName = _commandName,
            HandlerName = _handlerName
        };

        WriteFile(templatePath, model, _handlerName);
    }
}