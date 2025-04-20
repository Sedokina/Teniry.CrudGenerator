using FluentAssertions.Execution;
using Teniry.CrudGenerator.Tests.Helpers;

namespace Teniry.CrudGenerator.Tests;

public class IncrementalGeneratorTests {
    [Fact]
    public Task Should_TakeCachedSources_OnSecondRun() {
        // Arrange
        const string source = """
            using Microsoft.EntityFrameworkCore;
            using Teniry.CrudGenerator.Abstractions.DbContext;
            using Teniry.CrudGenerator.Abstractions.Configuration;

            namespace Teniry.CrudGenerator.Tests;

            public class TestEntity {
                   public int Id { get; set; }
                   public string Name { get; set; }
            }

            public class TestEntityGeneratorConfiguration : EntityGeneratorConfiguration<TestEntity> {}

            [UseDbContext(DbContextDbProvider.Mongo)]
            public class TestDb : DbContext {}
            """;

        // Act
        var (diagnostics, output) = CrudHelper.RunGenerator(source);

        // Assert
        using var scope = new AssertionScope();
        diagnostics.Should().BeEmpty();
        output.Should().HaveCount(26);

        return Verify(output);
    }
}