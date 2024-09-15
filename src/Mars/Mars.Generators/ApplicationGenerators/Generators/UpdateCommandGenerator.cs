using Mars.Generators.ApplicationGenerators.Core;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class UpdateCommandGenerator : BaseGenerator
{
    private readonly string _commandName;
    private readonly string _handlerName;

    public UpdateCommandGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        CrudGeneratorConfiguration configuration)
        : base(context, symbol, configuration, configuration.UpdateCommandCommandGenerator.FunctionName)
    {
        _commandName = Configuration.UpdateCommandCommandGenerator.CommandNameFormat.GetName(_entityName);
        _handlerName = Configuration.UpdateCommandCommandGenerator.HandlerNameFormat.GetName(_entityName);
    }

    public void RunGenerator()
    {
        GenerateCommand(Configuration.UpdateCommandCommandGenerator.CommandTemplatePath);
        GenerateHandler(Configuration.UpdateCommandCommandGenerator.HandlerTemplatePath);
    }

    private void GenerateCommand(string templatePath)
    {
        var properties = PropertiesExtractor.GetAllPropertiesOfEntity(Symbol);
        var model = new
        {
            CommandName = _commandName,
            Properties = properties
        };

        WriteFile(templatePath, model, _commandName);
    }

    private void GenerateHandler(string templatePath)
    {
        var properties = PropertiesExtractor.GetPrimaryKeyNamesOfEntity(Symbol, "command");
        var model = new
        {
            CommandName = _commandName,
            HandlerName = _handlerName,
            FindProperties = string.Join(", ", properties)
        };

        WriteFile(templatePath, model, _handlerName);
    }
}