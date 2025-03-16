using Teniry.Cqrs.Extended.Exceptions;
using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.EntityIdNameFeature.UpdateEntityIdName;
using Teniry.CrudGenerator.SampleApi.Generators.CustomIds.EntityIdNameGenerator;
using Moq;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.EntityIdNameGeneratorHandlerTests;

public class UpdateEntityIdNameHandlerTests {
    private readonly UpdateEntityIdNameCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly UpdateEntityIdNameHandler _sut;

    public UpdateEntityIdNameHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid()) {
            Name = "New entity name"
        };
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_UpdatingNotExistingEntity() {
        // Arrange
        _db.Setup(
                x =>
                    x.FindAsync<EntityIdName>(new object[] { _command.EntityIdNameId }, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((EntityIdName?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new());

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>()
            .Where(x => x.NotFoundType == typeof(EntityIdName));
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave() {
        // Arrange
        var entity = new EntityIdName { EntityIdNameId = _command.EntityIdNameId, Name = "Old entity name" };
        _db.Setup(
                x =>
                    x.FindAsync<EntityIdName>(new object[] { _command.EntityIdNameId }, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(entity);

        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        entity.Name.Should().Be("New entity name");
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<EntityIdName>(new object[] { _command.EntityIdNameId }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }
}