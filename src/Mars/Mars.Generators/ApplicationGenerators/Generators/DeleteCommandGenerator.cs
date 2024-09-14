using System.Text;
using Mars.Generators.ApplicationGenerators.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class DeleteCommandGenerator : BaseGenerator
{
    private readonly string _commandName;
    private readonly string _handlerName;

    public DeleteCommandGenerator(GeneratorExecutionContext context, ISymbol symbol) : base(context, symbol)
    {
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

        var model = new
        {
            CommandName = _commandName,
            Properties = result
        };
        WriteFile(templatePath, model, _commandName);
    }

    private void GenerateHandler(string templatePath)
    {
        var properties = PropertiesExtractor.GetPrimaryKeysOfEntity(Symbol, "command");

        var model = new
        {
            CommandName = _commandName,
            HandlerName = _handlerName,
            FindProperties = string.Join(", ", properties)
        };
        WriteFile(templatePath, model, _handlerName);
    }
}