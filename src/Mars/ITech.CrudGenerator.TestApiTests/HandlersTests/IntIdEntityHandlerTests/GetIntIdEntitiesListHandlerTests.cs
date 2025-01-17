using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.IntIdEntityFeature.GetIntIdEntities;
using ITech.CrudGenerator.TestApi.Generators.IntIdEntityGenerator;
using Moq;
using Moq.EntityFrameworkCore;

namespace ITech.CrudGenerator.TestApiTests.HandlersTests.IntIdEntityHandlerTests;

public class GetIntIdEntitiesListHandlerTests {
    private readonly Mock<TestMongoDb> _db;
    private readonly GetIntIdEntitiesQuery _query;
    private readonly GetIntIdEntitiesHandler _sut;

    public GetIntIdEntitiesListHandlerTests() {
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
        _db.Setup(x => x.Set<IntIdEntity>())
            .ReturnsDbSet([new() { Id = 1, Name = "Test Entity" }]);

        // Act
        var entities = await _sut.HandleAsync(_query, new());

        // Assert
        entities.Page.Should().NotBeNull();
        entities.Page.CurrentPageIndex.Should().Be(1);
        entities.Page.PageSize.Should().Be(10);
        entities.Items.Should().SatisfyRespectively(
            dto => {
                dto.Id.Should().BeGreaterThan(0);
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
}