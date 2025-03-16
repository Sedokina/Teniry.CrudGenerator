using Teniry.Cqrs.Extended.Exceptions;
using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.EntityIdNameFeature.GetEntityIdName;
using Teniry.CrudGenerator.SampleApi.Generators.CustomIds.EntityIdNameGenerator;
using Moq;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.EntityIdNameGeneratorHandlerTests;

public class GetEntityIdNameHandlerTests {
    private readonly Mock<TestMongoDb> _db;
    private readonly GetEntityIdNameQuery _query;
    private readonly GetEntityIdNameHandler _sut;

    public GetEntityIdNameHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _query = new(Guid.NewGuid());
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_GettingNotExistingEntity() {
        // Arrange
        _db.Setup(x => x.FindAsync<EntityIdName>(new object[] { _query.EntityIdNameId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((EntityIdName?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_query, new());

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>()
            .Where(x => x.NotFoundType == typeof(EntityIdName));
    }

    [Fact]
    public async Task Should_GetEntityWithCorrectData() {
        // Arrange
        _db.Setup(x => x.FindAsync<EntityIdName>(new object[] { _query.EntityIdNameId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EntityIdName { EntityIdNameId = _query.EntityIdNameId, Name = "My test entity" });

        // Act
        var entity = await _sut.HandleAsync(_query, new());

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