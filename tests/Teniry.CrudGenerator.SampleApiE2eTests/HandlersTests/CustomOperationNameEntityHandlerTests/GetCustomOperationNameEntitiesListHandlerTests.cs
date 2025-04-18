using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.CustomOperationNameEntityFeature.
    CustomOpGetListCustomOperationNameEntities;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;
using Moq;
using Moq.EntityFrameworkCore;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomOperationNameEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.CustomOperationNameEntityHandlerTests;

public class GetCustomOperationNameEntitiesListHandlerTests {
    private readonly Mock<SampleMongoDb> _db;
    private readonly CustomOpGetListCustomOperationNameEntitiesQuery _query;
    private readonly CustomOpGetListCustomOperationNameEntitiesHandler _sut;

    public GetCustomOperationNameEntitiesListHandlerTests() {
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
        _db.Setup(x => x.Set<CustomOperationNameEntity>())
            .ReturnsDbSet([new() { Id = Guid.NewGuid(), Name = "Test Entity" }]);

        // Act
        var entities = await _sut.HandleAsync(_query, new());

        // Assert
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
    [InlineData("CustomOpGetListCustomOperationNameEntitiesQuery")]
    [InlineData("CustomOpGetListCustomOperationNameEntitiesHandler")]
    [InlineData("CustomOperationNameEntitiesDto")]
    public void Should_BeInOperationNamespace(string typeName) {
        // Assert
        typeof(Program).Assembly.Should()
            .BeInNamespaceThatEndsWith(typeName, "CustomOpGetListCustomOperationNameEntities");
    }
}