using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Teniry.CrudGenerator.Core.Configurations.Global;
using Teniry.CrudGenerator.Core.Configurations.Shared;
using Teniry.CrudGenerator.Core.Generators;
using Teniry.CrudGenerator.Core.Generators.Core;
using Teniry.CrudGenerator.Core.Runners;
using Teniry.CrudGenerator.Core.Schemes.DbContext;
using Teniry.CrudGenerator.Core.Schemes.Entity;
using Teniry.CrudGenerator.Core.Schemes.InternalEntityGenerator;
using Teniry.CrudGenerator.Diagnostics;

namespace Teniry.CrudGenerator;

[Generator]
public sealed class CrudGenerator : IIncrementalGenerator {
    // review todos
    // reformat code with customized formatter
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var generatorConfigurations = ValueProviders.GetGeneratorConfigurations(context);
        var dbContexts = ValueProviders.GetDbContexts(context);
        var dbContext = dbContexts.Collect();

        List<EndpointMap> endpointsMaps = [];
        var globalConfiguration = GlobalCrudGeneratorConfigurationFactory.Construct();
        var sharedConfigurationBuilder = new CqrsOperationsSharedConfiguratorFactory().Construct();

        var generatorRunners = generatorConfigurations
            .Combine(dbContext)
            .WithTrackingName(CrudGeneratorTrackingNames.CombineConfigurationsAndDbContexts)
            .Where(x => x.Left.Value is not null && x.Right.Length > 0)
            .Select((tuple, _) => SelectConfigurations(tuple.Left, tuple.Right))
            .WithTrackingName(CrudGeneratorTrackingNames.TransformConfigurationsAndDbContexts)
            .SelectMany(
                (tuple, _) => CreateGeneratorRunners(
                    globalConfiguration,
                    sharedConfigurationBuilder,
                    tuple.EntityGeneratorConfiguration,
                    tuple.EntityScheme,
                    tuple.DbContextScheme
                )
            )
            .WithTrackingName(CrudGeneratorTrackingNames.CreateGeneratorRunners);

        context.RegisterSourceOutput(
            dbContext,
            (productionContext, dbContextSchemesResult) => ReportDiagnostics(dbContextSchemesResult, productionContext)
        );

        context.RegisterSourceOutput(
            generatorRunners,
            (productionContext, generatorRunner) =>
                BuildCrudOperations(productionContext, generatorRunner, endpointsMaps)
        );

        context.RegisterSourceOutput(
            dbContext,
            (productionContext, _) => { BuildEndpointsMap(endpointsMaps, globalConfiguration, productionContext); }
        );
    }

    private static IEnumerable<IGeneratorRunner> CreateGeneratorRunners(
        GlobalCrudGeneratorConfiguration globalConfiguration,
        CqrsOperationsSharedConfigurator sharedConfigurationBuilder,
        InternalEntityGeneratorConfiguration entityGeneratorConfiguration,
        EntityScheme entityScheme,
        DbContextScheme dbContextScheme
    ) {
        var getByIdQueryGeneratorRunner = new GetByIdQueryGeneratorRunner(
            globalConfiguration,
            sharedConfigurationBuilder,
            entityGeneratorConfiguration.GetByIdOperation,
            entityScheme,
            dbContextScheme
        );

        var getByIdRouteBuilder = getByIdQueryGeneratorRunner.Configuration.Endpoint.Generate ?
            GetByIdQueryGeneratorRunner
                .GetRouteConfigurationBuilder(entityGeneratorConfiguration.GetByIdOperation) :
            null;

        return [
            getByIdQueryGeneratorRunner,

            new GetListQueryGeneratorRunner(
                globalConfiguration,
                sharedConfigurationBuilder,
                entityGeneratorConfiguration.GetListOperation,
                entityScheme,
                dbContextScheme
            ),
            new CreateCommandGeneratorRunner(
                globalConfiguration,
                sharedConfigurationBuilder,
                entityGeneratorConfiguration.CreateOperation,
                entityScheme,
                dbContextScheme,
                getByIdRouteBuilder,
                getByIdQueryGeneratorRunner.Configuration.OperationName
            ),
            new UpdateCommandGeneratorRunner(
                globalConfiguration,
                sharedConfigurationBuilder,
                entityGeneratorConfiguration.UpdateOperation,
                entityScheme,
                dbContextScheme
            ),
            new PatchCommandGeneratorRunner(
                globalConfiguration,
                sharedConfigurationBuilder,
                entityGeneratorConfiguration.PatchOperation,
                entityScheme,
                dbContextScheme
            ),
            new DeleteCommandGeneratorRunner(
                globalConfiguration,
                sharedConfigurationBuilder,
                entityGeneratorConfiguration.DeleteOperation,
                entityScheme,
                dbContextScheme
            )
        ];
    }

    private static (
        InternalEntityGeneratorConfiguration EntityGeneratorConfiguration,
        EntityScheme EntityScheme,
        DbContextScheme DbContextScheme
        )
        SelectConfigurations(
            Result<InternalEntityGeneratorConfiguration?> generatorConfiguration,
            ImmutableArray<Result<DbContextScheme>> dbContextSchemes
        ) {
        return (
            EntityGeneratorConfiguration: generatorConfiguration.Value!,
            EntityScheme: EntitySchemeFactory.Construct(generatorConfiguration.Value!, dbContextSchemes[0].Value),
            DbContextScheme: dbContextSchemes[0].Value
        );
    }

    private static void BuildCrudOperations(
        SourceProductionContext context,
        IGeneratorRunner generatorRunner,
        List<EndpointMap> endpointsMaps
    ) {
        var generatedFiles = generatorRunner.RunGenerator(endpointsMaps);
        WriteFiles(context, generatedFiles);
    }

    private static void BuildEndpointsMap(
        List<EndpointMap> endpointsMaps,
        GlobalCrudGeneratorConfiguration globalConfiguration,
        SourceProductionContext productionContext
    ) {
        var mapEndpointsGenerator = new EndpointsMapGenerator(endpointsMaps, globalConfiguration);
        mapEndpointsGenerator.RunGenerator();
        WriteFiles(productionContext, mapEndpointsGenerator.GeneratedFiles);
    }

    private static void WriteFiles(SourceProductionContext context, List<GeneratorResult> files) {
        foreach (var file in files) {
            context.AddSource(file.FileName, file.Source);
        }
    }

    private static void ReportDiagnostics(
        ImmutableArray<Result<DbContextScheme>> dbContextSchemesResult,
        SourceProductionContext productionContext
    ) {
        if (dbContextSchemesResult.Length == 0) {
            var dbContextNotFoundDiagnostics = Diagnostic.Create(DiagnosticDescriptors.DbContextNotFound, null);
            productionContext.ReportDiagnostic(dbContextNotFoundDiagnostics);
        }

        var diagnostics = dbContextSchemesResult
            .SelectMany(x => x.Diagnostics.Select(c => Diagnostic.Create(c.Descriptor, null, c.Location!.FilePath)));
        foreach (var diagnostic in diagnostics) {
            productionContext.ReportDiagnostic(diagnostic);
        }
    }
}