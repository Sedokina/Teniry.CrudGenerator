using FluentAssertions.Execution;
using Teniry.CrudGenerator.Tests.Helpers;

namespace Teniry.CrudGenerator.Tests;

public class IncrementalGeneratorTests {
    private const string Source = """
        using Microsoft.EntityFrameworkCore;
        using Teniry.CrudGenerator.Abstractions.DbContext;
        using Teniry.CrudGenerator.Abstractions.Configuration;

        namespace Teniry.CrudGenerator.Tests;

        public class TestEntity {
               public int Id { get; set; }
               public string Name { get; set; }
        }

        [UseDbContext(DbContextDbProvider.Mongo)]
        public class TestDb : DbContext {}
        """;

    [Fact]
    public void Should_TakeCachedSources_OnSecondRun() {
        // Act
        var (diagnostics, output) = CrudHelper.RunGeneratorIncrementaly(Source);

        // Assert
        using var scope = new AssertionScope();
        diagnostics.Should().BeEmpty();
        output.Should().HaveCount(26);
    }

    [Fact]
    public Task Should_Correctly_GenerateFiles() {
        // Assert
        return CrudHelper.Verify(Source);
    }
}