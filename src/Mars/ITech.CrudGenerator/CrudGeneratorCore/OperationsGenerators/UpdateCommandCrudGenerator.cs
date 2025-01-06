using System.Linq;
using System.Threading;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.Formatters;
using Microsoft.CodeAnalysis.CSharp;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators;

internal class UpdateCommandCrudGenerator
    : BaseOperationCrudGenerator<CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration>
{
    private readonly string _commandName;
    private readonly string _handlerName;
    private readonly string _vmName;
    private readonly string _endpointClassName;

    public UpdateCommandCrudGenerator(
        CrudGeneratorScheme<CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration> scheme)
        : base(scheme)
    {
        _commandName = scheme.Configuration.Operation;
        _handlerName = scheme.Configuration.Handler;
        _vmName = scheme.Configuration.ViewModel;
        _endpointClassName = scheme.Configuration.Endpoint.Name;
    }

    public override void RunGenerator()
    {
        GenerateCommand();
        GenerateHandler();
        GenerateViewModel();
        if (Scheme.Configuration.Endpoint.Generate)
        {
            GenerateEndpoint();
        }
    }

    private void GenerateCommand()
    {
        var command = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.PartialKeyword
            ], _commandName)
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation)
            .WithUsings(["ITech.Cqrs.Domain.Exceptions"])
            .WithXmlDoc($"Update {EntityScheme.EntityTitle}", "Nothing",
            [
                new XmlDocException(
                    "EfEntityNotFoundException",
                    $"When {Scheme.EntityScheme.EntityTitle} entity does not exist"
                )
            ]);

        var constructorParameters = EntityScheme.PrimaryKeys
            .Select(x => new ParameterOfMethodBuilder(x.TypeName, x.PropertyNameAsMethodParameterName)).ToList();
        var constructor = new ConstructorBuilder([SyntaxKind.PublicKeyword], _commandName)
            .WithParameters(constructorParameters);
        var constructorBody = new MethodBodyBuilder();
        foreach (var primaryKey in EntityScheme.PrimaryKeys)
        {
            command.WithProperty(primaryKey.TypeName, primaryKey.PropertyName);
            constructorBody.AssignVariable(primaryKey.PropertyName, primaryKey.PropertyNameAsMethodParameterName);
        }

        foreach (var property in EntityScheme.NotPrimaryKeys)
        {
            command.WithProperty(property.TypeName, property.PropertyName, property.DefaultValue);
        }

        constructor.WithBody(constructorBody.Build());
        command.WithConstructor(constructor.Build());

        WriteFile(_commandName, command.BuildAsString());
    }

    private void GenerateHandler()
    {
        var handlerClass = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.PartialKeyword
            ], _handlerName)
            .WithUsings([
                "ITech.Cqrs.Cqrs.Commands",
                "ITech.Cqrs.Domain.Exceptions",
                Scheme.DbContextScheme.DbContextNamespace,
                EntityScheme.EntityNamespace,
                "Mapster",
            ])
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation)
            .Implements("ICommandHandler", _commandName)
            .WithPrivateField([SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword],
                Scheme.DbContextScheme.DbContextName, "_db");

        var constructor = new ConstructorBuilder([SyntaxKind.PublicKeyword], _handlerName)
            .WithParameters([new ParameterOfMethodBuilder(Scheme.DbContextScheme.DbContextName, "db")]);
        var constructorBody = new MethodBodyBuilder()
            .AssignVariable("_db", "db");

        constructor.WithBody(constructorBody.Build());

        var methodBuilder = new MethodBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.AsyncKeyword
            ], "Task", "HandleAsync")
            .WithParameters([
                new ParameterOfMethodBuilder(_commandName, "command"),
                new ParameterOfMethodBuilder(nameof(CancellationToken), "cancellation")
            ])
            .WithXmlInheritdoc();

        var findParameters = EntityScheme.PrimaryKeys.GetAsMethodCallParameters("command");
        var methodBodyBuilder = new MethodBodyBuilder()
            .InitArrayVariable("object", "entityIds", findParameters)
            .InitVariableFromGenericAsyncMethodCall("entity", "_db", "FindAsync",
                [EntityScheme.EntityName.ToString()],
                ["entityIds", "cancellation"])
            .ThrowIfEntityNotFound("entity", EntityScheme.EntityName.ToString())
            .CallMethod("command", "Adapt", ["entity"])
            .CallAsyncMethod("_db", "SaveChangesAsync", ["cancellation"]);

        methodBuilder.WithBody(methodBodyBuilder.Build());
        handlerClass.WithConstructor(constructor.Build());
        handlerClass.WithMethod(methodBuilder.Build());

        WriteFile(_handlerName, handlerClass.BuildAsString());
    }

    private void GenerateViewModel()
    {
        var vmClass = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.PartialKeyword
            ], _vmName)
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature);

        foreach (var property in EntityScheme.NotPrimaryKeys)
        {
            vmClass.WithProperty(property.TypeName, property.PropertyName, property.DefaultValue);
        }

        WriteFile(_vmName, vmClass.BuildAsString());
    }

    private void GenerateEndpoint()
    {
        var endpointClass = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.StaticKeyword,
                SyntaxKind.PartialKeyword
            ], _endpointClassName)
            .WithUsings([
                "Microsoft.AspNetCore.Mvc",
                "ITech.Cqrs.Cqrs.Commands",
                "Mapster",
                Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation
            ])
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature);

        var methodBuilder = new MethodBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.StaticKeyword,
                SyntaxKind.AsyncKeyword
            ], "Task<IResult>", Scheme.Configuration.Endpoint.FunctionName)
            .WithParameters(EntityScheme.PrimaryKeys
                .Select(x => new ParameterOfMethodBuilder(x.TypeName, x.PropertyNameAsMethodParameterName))
                .Append(new ParameterOfMethodBuilder(_vmName, "vm"))
                .Append(new ParameterOfMethodBuilder("ICommandDispatcher", "commandDispatcher"))
                .Append(new ParameterOfMethodBuilder("CancellationToken", "cancellation"))
                .ToList())
            .WithProducesResponseTypeAttribute(204)
            .WithXmlDoc($"Update {Scheme.EntityScheme.EntityTitle}",
                204,
                $"{Scheme.EntityScheme.EntityTitle} updated");

        var methodBodyBuilder = new MethodBodyBuilder()
            .InitVariableFromConstructorCall("command", _commandName, EntityScheme.PrimaryKeys)
            .CallMethod("vm", "Adapt", ["command"])
            .CallGenericAsyncMethod("commandDispatcher", "DispatchAsync", [_commandName], ["command", "cancellation"])
            .ReturnTypedResultNoContent();

        methodBuilder.WithBody(methodBodyBuilder.Build());
        endpointClass.WithMethod(methodBuilder.Build());

        WriteFile(_endpointClassName, endpointClass.BuildAsString());

        EndpointMap = new EndpointMap(EntityScheme.EntityTitle.ToString(),
            Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature,
            "Put",
            Scheme.Configuration.Endpoint.Route,
            _endpointClassName,
            Scheme.Configuration.Endpoint.FunctionName);
    }
}