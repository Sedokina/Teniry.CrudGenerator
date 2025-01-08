using System.Collections.Generic;
using System.Linq;
using ITech.CrudGenerator.Abstractions.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global.Factories;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator;

[Generator]
public sealed class CrudGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var generatorConfigurationsProviders = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: (node, _) => CheckIfNodeIsInheritedFromClass(node, "EntityGeneratorConfiguration"),
            transform: (syntaxContext, _) => TransformFoundGeneratorConfigurationsToInternalScheme(syntaxContext)
        );

        var dbContextSchemeProviders = context.SyntaxProvider.ForAttributeWithMetadataName(
            typeof(UseDbContextAttribute).FullName ?? "",
            predicate: (node, token) => true,
            transform: (syntaxContext, token) => DbContextSchemeFactory.Construct(syntaxContext));

        var generatorConfigurationsWithDbContextProviders =
            generatorConfigurationsProviders.Combine(dbContextSchemeProviders.Collect());

        var r = generatorConfigurationsProviders
            .Select((configuration, token) => new EndpointMap(configuration.ClassMetadata.ClassName, "some", "things",
                "gotta", "give", "well"))
            .Collect();

        List<EndpointMap> endpointsMaps = new();
        var globalConfigurationBuilder = GlobalCrudGeneratorConfigurationDefaultConfigurationFactory.Construct();
        var sharedConfigurationBuilder = new CqrsOperationsSharedConfigurationBuilderFactory().Construct();

        // TODO: check if there are dbContextSchemes at all
        context.RegisterSourceOutput(generatorConfigurationsWithDbContextProviders,
            (productionContext, generatorConfigurationWithDbContext) =>
                Execute(
                    productionContext,
                    generatorConfigurationWithDbContext.Left,
                    generatorConfigurationWithDbContext.Right[0],
                    globalConfigurationBuilder,
                    sharedConfigurationBuilder,
                    endpointsMaps
                )
        );

        context.RegisterSourceOutput(r, (productionContext, maps) =>
        {
            var mapEndpointsGenerator = new EndpointsMapGenerator(endpointsMaps, globalConfigurationBuilder);
            mapEndpointsGenerator.RunGenerator();
            WriteFiles(productionContext, mapEndpointsGenerator.GeneratedFiles);
        });
    }

    private static bool CheckIfNodeIsInheritedFromClass(SyntaxNode node, string className)
    {
        return node is ClassDeclarationSyntax classDeclarationSyntax &&
               classDeclarationSyntax is { BaseList.Types.Count: > 0 } &&
               classDeclarationSyntax.BaseList.Types
                   .Any(x => x.Type is GenericNameSyntax baseClass &&
                             baseClass.Identifier.ToString().Equals(className));
    }

    private InternalEntityGeneratorConfiguration TransformFoundGeneratorConfigurationsToInternalScheme(
        GeneratorSyntaxContext syntaxContext
    )
    {
        var symbol = syntaxContext.SemanticModel.GetDeclaredSymbol(syntaxContext.Node) as INamedTypeSymbol;
        var result = InternalEntityGeneratorConfigurationFactory
            .Construct(symbol, syntaxContext.SemanticModel.Compilation);
        return result;
    }

    internal static void Execute(
        SourceProductionContext context,
        InternalEntityGeneratorConfiguration internalEntityGeneratorConfiguration,
        DbContextScheme dbContextScheme,
        GlobalCqrsGeneratorConfigurationBuilder globalConfigurationBuilder,
        CqrsOperationsSharedConfigurationBuilder sharedConfigurationBuilder,
        List<EndpointMap> endpointsMaps)
    {
        var entityScheme = new EntitySchemeFactory().Construct(
            internalEntityGeneratorConfiguration,
            dbContextScheme);

        var getByIdQueryConfigurationBuilder = new GetByIdQueryDefaultConfigurationBuilderFactory()
            .Construct(
                globalConfigurationBuilder,
                sharedConfigurationBuilder,
                internalEntityGeneratorConfiguration.GetByIdOperation);

        var getByIdQueryConfiguration = getByIdQueryConfigurationBuilder.Build(entityScheme);
        if (getByIdQueryConfiguration.Generate)
        {
            var getByIdQueryScheme =
                new CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration>(
                    entityScheme,
                    dbContextScheme,
                    getByIdQueryConfiguration);
            var generateGetByIdQuery = new GetByIdQueryCrudGenerator(getByIdQueryScheme);
            generateGetByIdQuery.RunGenerator();
            WriteFiles(context, generateGetByIdQuery.GeneratedFiles);
            if (generateGetByIdQuery.EndpointMap is not null)
            {
                endpointsMaps.Add(generateGetByIdQuery.EndpointMap);
            }
        }

        var getListConfiguration = new GetListQueryDefaultConfigurationBulderFactory()
            .Construct(
                globalConfigurationBuilder,
                sharedConfigurationBuilder,
                internalEntityGeneratorConfiguration.GetListOperation)
            .Build(entityScheme);
        if (getListConfiguration.Generate)
        {
            var getListQueryScheme = new CrudGeneratorScheme<CqrsListOperationGeneratorConfiguration>(
                entityScheme,
                dbContextScheme,
                getListConfiguration);
            var generateListQuery = new ListQueryCrudGenerator(getListQueryScheme);
            generateListQuery.RunGenerator();
            WriteFiles(context, generateListQuery.GeneratedFiles);
            if (generateListQuery.EndpointMap is not null)
            {
                endpointsMaps.Add(generateListQuery.EndpointMap);
            }
        }

        var createCommandConfiguration = new CreateCommandDefaultConfigurationBuilderFactory()
            .Construct(
                globalConfigurationBuilder,
                sharedConfigurationBuilder,
                internalEntityGeneratorConfiguration.CreateOperation)
            .Build(entityScheme);
        if (createCommandConfiguration.Generate)
        {
            var createCommandScheme =
                new CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration>(
                    entityScheme,
                    dbContextScheme,
                    createCommandConfiguration
                );
            var generateCreateCommand = new CreateCommandCrudGenerator(
                createCommandScheme,
                getByIdQueryConfiguration.Endpoint.Generate
                    ? getByIdQueryConfigurationBuilder.Endpoint.RouteConfigurationBuilder
                    : null,
                getByIdQueryConfiguration.Endpoint.Generate
                    ? getByIdQueryConfigurationBuilder.OperationName
                    : null);
            generateCreateCommand.RunGenerator();
            WriteFiles(context, generateCreateCommand.GeneratedFiles);
            if (generateCreateCommand.EndpointMap is not null)
            {
                endpointsMaps.Add(generateCreateCommand.EndpointMap);
            }
        }

        var updateOperationConfiguration = new UpdateCommandDefaultConfigurationBuilderFactory()
            .Construct(
                globalConfigurationBuilder,
                sharedConfigurationBuilder,
                internalEntityGeneratorConfiguration.UpdateOperation)
            .Build(entityScheme);
        if (updateOperationConfiguration.Generate)
        {
            var updateCommandScheme =
                new CrudGeneratorScheme<CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration>(
                    entityScheme,
                    dbContextScheme,
                    updateOperationConfiguration);
            var generateUpdateCommand = new UpdateCommandCrudGenerator(updateCommandScheme);
            generateUpdateCommand.RunGenerator();
            WriteFiles(context, generateUpdateCommand.GeneratedFiles);
            if (generateUpdateCommand.EndpointMap is not null)
            {
                endpointsMaps.Add(generateUpdateCommand.EndpointMap);
            }
        }

        var deleteCommandConfiguration = new DeleteCommandDefaultConfigurationBuilderFactory()
            .Construct(
                globalConfigurationBuilder,
                sharedConfigurationBuilder,
                internalEntityGeneratorConfiguration.DeleteOperation)
            .Build(entityScheme);
        if (deleteCommandConfiguration.Generate)
        {
            var deleteCommandScheme =
                new CrudGeneratorScheme<CqrsOperationWithoutReturnValueGeneratorConfiguration>(
                    entityScheme,
                    dbContextScheme,
                    deleteCommandConfiguration);
            var generateDeleteCommand = new DeleteCommandCrudGenerator(deleteCommandScheme);
            generateDeleteCommand.RunGenerator();
            WriteFiles(context, generateDeleteCommand.GeneratedFiles);
            if (generateDeleteCommand.EndpointMap is not null)
            {
                endpointsMaps.Add(generateDeleteCommand.EndpointMap);
            }
        }
    }

    // public void Initialize(GeneratorInitializationContext context)
    // {
    //     context.RegisterForSyntaxNotifications(() => new EntityGeneratorConfigurationSyntaxReceiver());
    // }

    // public void Execute(GeneratorExecutionContext context)
    // {
    //     try
    //     {
    //         if (context.SyntaxReceiver is not EntityGeneratorConfigurationSyntaxReceiver syntaxReceiver) return;
    //
    //         var dbContextScheme = DbContextSchemeFactory.Construct(context);
    //
    //         List<EndpointMap> endpointsMaps = new();
    //         var globalConfigurationBuilder = GlobalCrudGeneratorConfigurationDefaultConfigurationFactory.Construct();
    //         var sharedConfigurationBuilder = new CqrsOperationsSharedConfigurationBuilderFactory().Construct();
    //
    //         var internalEntityGeneratorConfigurationFactory = new InternalEntityGeneratorConfigurationFactory();
    //         foreach (var classSyntax in syntaxReceiver.ClassesForCrudGeneration)
    //         {
    //             var (entityGeneratorConfigurationSymbol, entitySymbol) = classSyntax.AsSymbol(context);
    //
    //             var internalEntityGeneratorConfiguration = internalEntityGeneratorConfigurationFactory
    //                 .Construct(entityGeneratorConfigurationSymbol as INamedTypeSymbol, context.Compilation);
    //             var entityScheme = new EntitySchemeFactory().Construct(
    //                 entitySymbol,
    //                 internalEntityGeneratorConfiguration,
    //                 dbContextScheme);
    //
    //             var getByIdQueryConfigurationBuilder = new GetByIdQueryDefaultConfigurationBuilderFactory()
    //                 .Construct(
    //                     globalConfigurationBuilder,
    //                     sharedConfigurationBuilder,
    //                     internalEntityGeneratorConfiguration.GetByIdOperation);
    //
    //             var getByIdQueryConfiguration = getByIdQueryConfigurationBuilder.Build(entityScheme);
    //             if (getByIdQueryConfiguration.Generate)
    //             {
    //                 var getByIdQueryScheme =
    //                     new CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration>(
    //                         entityScheme,
    //                         dbContextScheme,
    //                         getByIdQueryConfiguration);
    //                 var generateGetByIdQuery = new GetByIdQueryCrudGenerator(getByIdQueryScheme);
    //                 generateGetByIdQuery.RunGenerator();
    //                 WriteFiles(context, generateGetByIdQuery.GeneratedFiles);
    //                 if (generateGetByIdQuery.EndpointMap is not null)
    //                 {
    //                     endpointsMaps.Add(generateGetByIdQuery.EndpointMap);
    //                 }
    //             }
    //
    //             var getListConfiguration = new GetListQueryDefaultConfigurationBulderFactory()
    //                 .Construct(
    //                     globalConfigurationBuilder,
    //                     sharedConfigurationBuilder,
    //                     internalEntityGeneratorConfiguration.GetListOperation)
    //                 .Build(entityScheme);
    //             if (getListConfiguration.Generate)
    //             {
    //                 var getListQueryScheme = new CrudGeneratorScheme<CqrsListOperationGeneratorConfiguration>(
    //                     entityScheme,
    //                     dbContextScheme,
    //                     getListConfiguration);
    //                 var generateListQuery = new ListQueryCrudGenerator(getListQueryScheme);
    //                 generateListQuery.RunGenerator();
    //                 WriteFiles(context, generateListQuery.GeneratedFiles);
    //                 if (generateListQuery.EndpointMap is not null)
    //                 {
    //                     endpointsMaps.Add(generateListQuery.EndpointMap);
    //                 }
    //             }
    //
    //             var createCommandConfiguration = new CreateCommandDefaultConfigurationBuilderFactory()
    //                 .Construct(
    //                     globalConfigurationBuilder,
    //                     sharedConfigurationBuilder,
    //                     internalEntityGeneratorConfiguration.CreateOperation)
    //                 .Build(entityScheme);
    //             if (createCommandConfiguration.Generate)
    //             {
    //                 var createCommandScheme =
    //                     new CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration>(
    //                         entityScheme,
    //                         dbContextScheme,
    //                         createCommandConfiguration
    //                     );
    //                 var generateCreateCommand = new CreateCommandCrudGenerator(
    //                     createCommandScheme,
    //                     getByIdQueryConfiguration.Endpoint.Generate
    //                         ? getByIdQueryConfigurationBuilder.Endpoint.RouteConfigurationBuilder
    //                         : null,
    //                     getByIdQueryConfiguration.Endpoint.Generate
    //                         ? getByIdQueryConfigurationBuilder.OperationName
    //                         : null);
    //                 generateCreateCommand.RunGenerator();
    //                 WriteFiles(context, generateCreateCommand.GeneratedFiles);
    //                 if (generateCreateCommand.EndpointMap is not null)
    //                 {
    //                     endpointsMaps.Add(generateCreateCommand.EndpointMap);
    //                 }
    //             }
    //
    //             var updateOperationConfiguration = new UpdateCommandDefaultConfigurationBuilderFactory()
    //                 .Construct(
    //                     globalConfigurationBuilder,
    //                     sharedConfigurationBuilder,
    //                     internalEntityGeneratorConfiguration.UpdateOperation)
    //                 .Build(entityScheme);
    //             if (updateOperationConfiguration.Generate)
    //             {
    //                 var updateCommandScheme =
    //                     new CrudGeneratorScheme<CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration>(
    //                         entityScheme,
    //                         dbContextScheme,
    //                         updateOperationConfiguration);
    //                 var generateUpdateCommand = new UpdateCommandCrudGenerator(updateCommandScheme);
    //                 generateUpdateCommand.RunGenerator();
    //                 WriteFiles(context, generateUpdateCommand.GeneratedFiles);
    //                 if (generateUpdateCommand.EndpointMap is not null)
    //                 {
    //                     endpointsMaps.Add(generateUpdateCommand.EndpointMap);
    //                 }
    //             }
    //
    //             var deleteCommandConfiguration = new DeleteCommandDefaultConfigurationBuilderFactory()
    //                 .Construct(
    //                     globalConfigurationBuilder,
    //                     sharedConfigurationBuilder,
    //                     internalEntityGeneratorConfiguration.DeleteOperation)
    //                 .Build(entityScheme);
    //             if (deleteCommandConfiguration.Generate)
    //             {
    //                 var deleteCommandScheme =
    //                     new CrudGeneratorScheme<CqrsOperationWithoutReturnValueGeneratorConfiguration>(
    //                         entityScheme,
    //                         dbContextScheme,
    //                         deleteCommandConfiguration);
    //                 var generateDeleteCommand = new DeleteCommandCrudGenerator(deleteCommandScheme);
    //                 generateDeleteCommand.RunGenerator();
    //                 WriteFiles(context, generateDeleteCommand.GeneratedFiles);
    //                 if (generateDeleteCommand.EndpointMap is not null)
    //                 {
    //                     endpointsMaps.Add(generateDeleteCommand.EndpointMap);
    //                 }
    //             }
    //         }
    //
    //         var mapEndpointsGenerator = new EndpointsMapGenerator(endpointsMaps, globalConfigurationBuilder);
    //         mapEndpointsGenerator.RunGenerator();
    //         WriteFiles(context, mapEndpointsGenerator.GeneratedFiles);
    //     }
    //     catch (Exception e)
    //     {
    //         Logger.Print($"{e.Message}\n{e.StackTrace}");
    //         Logger.FlushLogs(context);
    //     }
    // }

    private static void WriteFiles(SourceProductionContext context, List<GeneratorResult> files)
    {
        foreach (var file in files)
        {
            context.AddSource(file.FileName, file.Source);
        }
    }
}

internal class CrudGeneratorScheme<TConfiguration>
    where TConfiguration : CqrsOperationWithoutReturnValueGeneratorConfiguration
{
    public EntityScheme EntityScheme { get; set; }
    public DbContextScheme DbContextScheme { get; set; }
    public TConfiguration Configuration { get; set; }

    public CrudGeneratorScheme(EntityScheme entityScheme, DbContextScheme dbContextScheme, TConfiguration configuration)
    {
        EntityScheme = entityScheme;
        DbContextScheme = dbContextScheme;
        Configuration = configuration;
    }
}