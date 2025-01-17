using ITech.Cqrs.Domain.Exceptions;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.EntityIdNameFeature.GetEntityIdName;
using ITech.CrudGenerator.TestApi.Generators.CustomIds.EntityIdNameGenerator;
using Moq;

namespace ITech.CrudGenerator.TestApiTests.HandlersTests.EntityIdNameGeneratorHandlerTests;

public class GetEntityIdNameHandlerTests {
    private readonly Mock<TestMongoDb> _db;
    private readonly GetEntityIdNameQuery _query;
    private readonly GetEntityIdNameHandler _sut;

    public GetEntityIdNameHandlerTests() {
        _db = new Mock<TestMongoDb>();
        _sut = new(_db.Object);
        _query = new(Guid.NewGuid());
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_GettingNotExistingEntity() {
        // Arrange
        _db.Setup(x => x.FindAsync<EntityIdName>(new object[] { _query.EntityIdNameId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((EntityIdName?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_query, new CancellationToken());

        // Assert
        await act.Should().ThrowAsync<EfEntityNotFoundException>()
            .Where(x => x.TypeName.Equals(nameof(EntityIdName)));
    }

    [Fact]
    public async Task Should_GetEntityWithCorrectData() {
        // Arrange
        _db.Setup(x => x.FindAsync<EntityIdName>(new object[] { _query.EntityIdNameId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EntityIdName { EntityIdNameId = _query.EntityIdNameId, Name = "My test entity" });

        // Act
        var entity = await _sut.HandleAsync(_query, new CancellationToken());

        // Assert
        entity.EntityIdNameId.Should().Be(_query.EntityIdNameId);
        entity.Name.Should().Be("My test entity");
        _db.Verify(
            x => x.FindAsync<EntityIdName>(new object[] { _query.EntityIdNameId }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }
}