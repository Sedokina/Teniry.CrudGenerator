using ITech.Cqrs.Domain.Exceptions;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.SimpleEntityFeature.GetSimpleEntity;
using ITech.CrudGenerator.TestApi.Generators.SimpleEntityGenerator;
using Moq;

namespace ITech.CrudGenerator.TestApiTests.HandlersTests.SimpleEntityHandlersTests;

public class GetSimpleEntityHandlerTests {
    private readonly Mock<TestMongoDb> _db;
    private readonly GetSimpleEntityQuery _query;
    private readonly GetSimpleEntityHandler _sut;

    public GetSimpleEntityHandlerTests() {
        _db = new Mock<TestMongoDb>();
        _sut = new(_db.Object);
        _query = new(Guid.NewGuid());
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_GettingNotExistingEntity() {
        // Arrange
        _db.Setup(x => x.FindAsync<SimpleEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SimpleEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_query, new CancellationToken());

        // Assert
        await act.Should().ThrowAsync<EfEntityNotFoundException>()
            .Where(x => x.TypeName.Equals(nameof(SimpleEntity)));
    }

    [Fact]
    public async Task Should_GetEntityWithCorrectData() {
        // Arrange
        _db.Setup(x => x.FindAsync<SimpleEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SimpleEntity { Id = _query.Id, Name = "My test entity" });

        // Act
        var entity = await _sut.HandleAsync(_query, new CancellationToken());

        // Assert
        entity.Id.Should().Be(_query.Id);
        entity.Name.Should().Be("My test entity");
        _db.Verify(
            x => x.FindAsync<SimpleEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }
}