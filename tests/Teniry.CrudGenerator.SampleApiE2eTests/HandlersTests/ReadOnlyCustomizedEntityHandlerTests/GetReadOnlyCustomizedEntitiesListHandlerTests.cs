using Moq;
using Moq.EntityFrameworkCore;
using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.ReadOnlyCustomizedEntityFeature.GetReadOnlyModelsListCustomNamespace;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.ReadOnlyCustomizedEntityGenerator;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.ReadOnlyCustomizedEntityHandlerTests;

public class GetReadOnlyCustomizedEntitiesListHandlerTests {
    private readonly Mock<SampleMongoDb> _db;
    private readonly GetReadOnlyModelsQuery _query;
    private readonly GetReadOnlyModelsHandler _sut;

    public GetReadOnlyCustomizedEntitiesListHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _query = new() {
            Name = "Test Entity",
            Sort = ["id", "name"],
            Page = 1,
            PageSize = 10
        };
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave() {
        // Arrange
        _db.Setup(x => x.Set<ReadOnlyCustomizedEntity>())
            .ReturnsDbSet([new() { Id = Guid.NewGuid(), Name = "Test Entity" }]);

        // Act
        var entities = await _sut.HandleAsync(_query, new());

        // Assert
        entities.Should().BeOfType<ReadOnlyModelsListCustomDto>();
        entities.Page.Should().NotBeNull();
        entities.Page.CurrentPageIndex.Should().Be(1);
        entities.Page.PageSize.Should().Be(10);
        entities.Items.Should().SatisfyRespectively(
            dto => {
                dto.Id.Should().NotBeEmpty();
                dto.Name.Should().NotBeEmpty();
            }
        );
    }

    [Fact]
    public void Should_HaveCorrectSortKeys() {
        // Assert
        _query.GetSortKeys()
            .Should().ContainInConsecutiveOrder("id", "name");
    }

    [Theory]
    [InlineData("GetReadOnlyModelsQuery")]
    [InlineData("GetReadOnlyModelsHandler")]
    [InlineData("ReadOnlyModelsListCustomDto")]
    public void Should_BeInCustomNamespace(string typeName) {
        // Assert
        typeof(Program).Assembly.Should()
            .BeInNamespaceThatEndsWith(typeName, "GetReadOnlyModelsListCustomNamespace");
    }
}