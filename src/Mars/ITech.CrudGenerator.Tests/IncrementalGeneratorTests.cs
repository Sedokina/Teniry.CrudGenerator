using FluentAssertions.Execution;
using ITech.CrudGenerator.Abstractions.DbContext;

namespace ITech.CrudGenerator.Tests;

public class IncrementalGeneratorTests
{
    // A collection of all the tracking names. I'll show how to simplify this later
    private static string[] AllTrackingNames =
    [
        "GeneratorConfigurationsProviders",
        "DbContextSchemeProviders",
        "generatorConfigurationsWithDbContextProviders",
        "entitySchemeFactoryWithDbContextProviders"
    ];

    [Fact]
    public void CanGenerate()
    {
        // Temporary, to ensure ITech.CrudGenerator.Abstractions are loaded for GetGeneratedTrees
        var d = DbContextDbProvider.Mongo;

        const string input = """
                             using ITech.CrudGenerator.Abstractions.DbContext;
                             using ITech.CrudGenerator.Abstractions.Configuration;

                             namespace ITech.CrudGenerator.Tests;

                             public class TestEntity {
                                    public int Id { get; set; }
                                    public string Name { get; set; }
                             }

                             public class TestEntityGeneratorConfiguration : EntityGeneratorConfiguration<TestEntity> {}

                             [UseDbContext(DbContextDbProvider.Mongo)]
                             public class TestDb : DbContext {}
                             """;

        // run the generator, passing in the inputs and the tracking names
        var (diagnostics, output)
            = TestHelpers.GetGeneratedTrees<CrudGenerator>([input], AllTrackingNames);

        // Assert the output
        using var scope = new AssertionScope();
        diagnostics.Should().BeEmpty();
        output.Should().HaveCount(22);
    }
}