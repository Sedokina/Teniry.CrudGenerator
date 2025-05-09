using Teniry.Cqrs.Extended.Exceptions;
using Teniry.CrudGenerator.SampleApi;
using Moq;
using Teniry.Cqrs.Extended.Types.PatchOperationType;
using Teniry.CrudGenerator.SampleApi.Application.GuidEntityFeature.PatchGuidEntity;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomIds.GuidEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.GuidEntityGeneratorHandlerTests;

public class PatchGuidEntityHandlerTests {
    private readonly PatchGuidEntityCommand _command;
    private readonly Mock<SampleMongoDb> _db;
    private readonly PatchGuidEntityHandler _sut;

    public PatchGuidEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid()) {
            Name = new("New entity name", PatchOpType.Update)
        };
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_UpdatingNotExistingEntity() {
        // Arrange
        _db.Setup(x => x.FindAsync<GuidEntity>(new object[] { _command.GuidEntityId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GuidEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>()
            .Where(x => x.NotFoundType == typeof(GuidEntity));
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave() {
        // Arrange
        var entity = new GuidEntity { GuidEntityId = _command.GuidEntityId, Name = "Old entity name" };
        _db.Setup(x => x.FindAsync<GuidEntity>(new object[] { _command.GuidEntityId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        await _sut.HandleAsync(_command, CancellationToken.None);

        // Assert
        entity.Name.Should().Be("New entity name");
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<GuidEntity>(new object[] { _command.GuidEntityId }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }
}