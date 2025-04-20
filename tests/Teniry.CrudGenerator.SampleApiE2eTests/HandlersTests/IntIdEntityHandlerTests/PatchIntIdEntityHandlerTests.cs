using Teniry.Cqrs.Extended.Exceptions;
using Teniry.CrudGenerator.SampleApi;
using Moq;
using Teniry.Cqrs.Extended.Types.PatchOperationType;
using Teniry.CrudGenerator.SampleApi.Application.IntIdEntityFeature.PatchIntIdEntity;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomIds.IntIdEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.IntIdEntityHandlerTests;

public class PatchIntIdEntityHandlerTests {
    private readonly PatchIntIdEntityCommand _command;
    private readonly Mock<SampleMongoDb> _db;
    private readonly PatchIntIdEntityHandler _sut;

    public PatchIntIdEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new(1) {
            Name = new("New entity name", PatchOpType.Update)
        };
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_UpdatingNotExistingEntity() {
        // Arrange
        _db.Setup(x => x.FindAsync<IntIdEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IntIdEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>()
            .Where(x => x.NotFoundType == typeof(IntIdEntity));
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave() {
        // Arrange
        var entity = new IntIdEntity { Id = _command.Id, Name = "Old entity name" };
        _db.Setup(x => x.FindAsync<IntIdEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        await _sut.HandleAsync(_command, CancellationToken.None);

        // Assert
        entity.Name.Should().Be("New entity name");
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<IntIdEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }
}