using System.Linq;
using System.Threading;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Crud;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core.SyntaxFactoryBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core.SyntaxFactoryBuilders.Models;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.Formatters;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core.SyntaxFactoryBuilders.SimpleSyntaxFactory;

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
        var constructor = new ConstructorBuilder(_commandName)
            .WithParameters(constructorParameters);
        var constructorBody = new BlockBuilder();
        foreach (var primaryKey in EntityScheme.PrimaryKeys)
        {
            command.WithProperty(primaryKey.TypeName, primaryKey.PropertyName);
            constructorBody.AssignVariable(primaryKey.PropertyName, primaryKey.PropertyNameAsMethodParameterName);
        }

        foreach (var property in EntityScheme.NotPrimaryKeys)
        {
            command.WithProperty(property.TypeName, property.PropertyName)
                .WithDefaultValue(property.DefaultValue);
        }

        constructor.WithBody(constructorBody);
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

        var constructor = new ConstructorBuilder(_handlerName)
            .WithParameters([new ParameterOfMethodBuilder(Scheme.DbContextScheme.DbContextName, "db")]);
        var constructorBody = new BlockBuilder()
            .AssignVariable("_db", "db");

        constructor.WithBody(constructorBody);

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
        var methodBodyBuilder = new BlockBuilder()
            .InitVariable("entity", CallGenericAsyncMethod(
                "_db",
                "FindAsync",
                [EntityScheme.EntityName.ToString()],
                [NewArray("object", findParameters), Variable("cancellation")])
            )
            .IfNull("entity", builder => builder.ThrowEntityNotFoundException(EntityScheme.EntityName.ToString()))
            .CallMethod("command", "Adapt", [Variable("entity")])
            .CallAsyncMethod("_db", "SaveChangesAsync", [Variable("cancellation")]);

        methodBuilder.WithBody(methodBodyBuilder);
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
            vmClass.WithProperty(property.TypeName, property.PropertyName)
                .WithDefaultValue(property.DefaultValue);
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
            .WithAttribute(new ProducesResponseTypeAttributeBuilder(204))
            .WithXmlDoc($"Update {Scheme.EntityScheme.EntityTitle}",
                204,
                $"{Scheme.EntityScheme.EntityTitle} updated");

        var methodBodyBuilder = new BlockBuilder()
            .InitVariable("command",
                CallConstructor(_commandName, EntityScheme.PrimaryKeys
                    .Select(x => Variable(x.PropertyNameAsMethodParameterName))
                    .ToList<ExpressionSyntax>()))
            .CallMethod("vm", "Adapt", [Variable("command")])
            .CallGenericAsyncMethod(
                "commandDispatcher",
                "DispatchAsync",
                [_commandName],
                [Variable("command"), Variable("cancellation")]
            )
            .Return(CallMethod("TypedResults", "NoContent", []));

        methodBuilder.WithBody(methodBodyBuilder);
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