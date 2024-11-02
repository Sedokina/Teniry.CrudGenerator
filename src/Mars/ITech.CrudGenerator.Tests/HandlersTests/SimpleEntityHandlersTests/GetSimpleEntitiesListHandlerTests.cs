using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.SimpleEntityFeature.GetSimpleEntities;
using ITech.CrudGenerator.TestApi.Generators.SimpleEntityGenerator;
using Moq;
using Moq.EntityFrameworkCore;

namespace ITech.CrudGenerator.Tests.HandlersTests.SimpleEntityHandlersTests;

public class GetSimpleEntitiesListHandlerTests
{
    private readonly Mock<TestMongoDb> _db;
    private readonly GetSimpleEntitiesQuery _query;
    private readonly GetSimpleEntitiesHandler _sut;

    public GetSimpleEntitiesListHandlerTests()
    {
        _db = new Mock<TestMongoDb>();
        _sut = new(_db.Object);
        _query = new()
        {
            Page = 1,
            PageSize = 10
        };
    }


    [Fact]
    public async Task Should_ChangeEntityDataAndSave()
    {
        // Arrange
        _db.Setup(x => x.Set<SimpleEntity>())
            .ReturnsDbSet([new SimpleEntity { Id = Guid.NewGuid(), Name = "My entity" }]);

        // Act
        var entities = await _sut.HandleAsync(_query, new CancellationToken());

        // Assert
        entities.Page.Should().NotBeNull();
        entities.Page.CurrentPageIndex.Should().Be(1);
        entities.Page.PageSize.Should().Be(10);
        entities.Items.Should().SatisfyRespectively(dto =>
        {
            dto.Id.Should().NotBeEmpty();
            dto.Name.Should().NotBeEmpty();
        });
    }
}