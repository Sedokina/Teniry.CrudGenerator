using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.ApplicationGenerators.Core.DbContextCore;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Formatters;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

internal class UpdateCommandCrudGenerator : BaseCrudGenerator<CqrsConfiguration>
{
    private readonly string _commandName;
    private readonly string _handlerName;
    private readonly string _vmName;
    private readonly string _endpointClassName;

    public UpdateCommandCrudGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        CqrsConfiguration configuration,
        EntityScheme entityScheme,
        DbContextScheme dbContextScheme) : base(context, symbol, configuration, entityScheme, dbContextScheme)
    {
        _commandName = entityScheme.Configuration.UpdateCommand.Operation.GetName(EntityScheme.EntityName);
        _handlerName = entityScheme.Configuration.UpdateCommand.Handler.GetName(EntityScheme.EntityName);
        _vmName = $"Update{EntityScheme.EntityName}Vm";
        _endpointClassName = entityScheme.Configuration.UpdateCommand.Endpoint.GetName(EntityScheme.EntityName);
    }

    public override void RunGenerator()
    {
        GenerateCommand(EntityScheme.Configuration.UpdateCommand.Operation.TemplatePath);
        GenerateHandler(EntityScheme.Configuration.UpdateCommand.Handler.TemplatePath);
        GenerateViewModel($"{EntityScheme.Configuration.TemplatesBasePath}.Update.UpdateVm.txt");
        GenerateEndpoint(EntityScheme.Configuration.UpdateCommand.Endpoint.TemplatePath);
    }

    private void GenerateCommand(string templatePath)
    {
        var properties = EntityScheme.Properties.FormatAsProperties();
        var constructorParameters = EntityScheme.PrimaryKeys.FormatAsMethodDeclarationParameters();
        var constructorBody = EntityScheme.PrimaryKeys.FormatAsConstructorBody();
        var model = new
        {
            CommandName = _commandName,
            Properties = properties,
            ConstructorParameters = constructorParameters,
            ConstructorBody = constructorBody
        };

        WriteFile(templatePath, model, _commandName);
    }

    private void GenerateHandler(string templatePath)
    {
        var findParameters = EntityScheme.PrimaryKeys.FormatAsMethodCallParameters("command");
        var model = new
        {
            CommandName = _commandName,
            HandlerName = _handlerName,
            FindParameters = findParameters
        };

        WriteFile(templatePath, model, _handlerName);
    }

    private void GenerateViewModel(string templatePath)
    {
        var properties = EntityScheme.NotPrimaryKeys.FormatAsProperties();
        var model = new
        {
            VmName = _vmName,
            Properties = properties,
        };

        WriteFile(templatePath, model, _vmName);
    }

    private void GenerateEndpoint(string templatePath)
    {
        var routeParams = EntityScheme.PrimaryKeys.FormatAsMethodDeclarationParameters();
        var constructorParameters = EntityScheme.PrimaryKeys.FormatAsMethodCallParameters();
        var model = new
        {
            EndpointClassName = _endpointClassName,
            RouteParams = routeParams,
            VmName = _vmName,
            CommandName = _commandName,
            CommandConstructorParameters = constructorParameters
        };

        WriteFile(templatePath, model, _endpointClassName);

        var constructorParametersForRoute = EntityScheme.PrimaryKeys.GetAsMethodCallParameters();
        EndpointMap = new EndpointMap(EntityScheme.EntityName.ToString(),
            EndpointNamespace,
            "Put",
            EntityScheme.Configuration.UpdateCommand.Endpoint.RouteConfiguration
                .GetRoute(EntityScheme.EntityName.ToString(), constructorParametersForRoute),
            $"{_endpointClassName}.{EntityScheme.Configuration.UpdateCommand.Endpoint.RouteConfiguration.FunctionName}");
    }
}