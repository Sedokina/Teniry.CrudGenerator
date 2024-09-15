using Mars.Generators.ApplicationGenerators.Core;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class DeleteCommandGenerator : BaseGenerator
{
    private readonly string _commandName;
    private readonly string _handlerName;

    public DeleteCommandGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        CrudGeneratorConfiguration configuration) 
        : base(context, symbol, configuration, configuration.DeleteCommandCommandGenerator.FunctionNameConfiguration)
    {
        _commandName = Configuration.DeleteCommandCommandGenerator.CommandNameConfiguration.GetName(_entityName);
        _handlerName = Configuration.DeleteCommandCommandGenerator.HandlerNameConfiguration.GetName(_entityName);
    }

    public void RunGenerator()
    {
        GenerateCommand(Configuration.DeleteCommandCommandGenerator.CommandTemplatePath);
        GenerateHandler(Configuration.DeleteCommandCommandGenerator.HandlerTemplatePath);
    }

    private void GenerateCommand(string templatePath)
    {
        var properties = PropertiesExtractor.GetPrimaryKeysOfEntityAsProperties(Symbol);
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