using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.GuidEntityFeature.DeleteGuidEntity;
using Moq;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomIds.GuidEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.GuidEntityGeneratorHandlerTests;

public class DeleteGuidEntityHandlerTests {
    private readonly DeleteGuidEntityCommand _command;
    private readonly Mock<SampleMongoDb> _db;
    private readonly DeleteGuidEntityHandler _sut;

    public DeleteGuidEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid());
    }

    [Fact]
    public async Task Should_DoNothingWhenEntityDoesNotExist() {
        // Arrange
        _db.Setup(
                x =>
                    x.FindAsync<GuidEntity>(new object[] { _command.GuidEntityId }, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync((GuidEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new());

        // Assert
        await act.Should().NotThrowAsync();
        _db.Verify(
            x => x.FindAsync<GuidEntity>(new object[] { _command.GuidEntityId }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Should_RemoveFromDbSetAndSave() {
        // Arrange
        _db.Setup(
                x =>
                    x.FindAsync<GuidEntity>(new object[] { _command.GuidEntityId }, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new GuidEntity { GuidEntityId = _command.GuidEntityId, Name = "Test entity" });

        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(x => x.Remove(It.IsAny<GuidEntity>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<GuidEntity>(new object[] { _command.GuidEntityId }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }
}