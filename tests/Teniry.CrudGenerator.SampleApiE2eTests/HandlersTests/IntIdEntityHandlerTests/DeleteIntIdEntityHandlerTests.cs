using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.IntIdEntityFeature.DeleteIntIdEntity;
using Moq;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomIds.IntIdEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.IntIdEntityHandlerTests;

public class DeleteIntIdEntityHandlerTests {
    private readonly DeleteIntIdEntityCommand _command;
    private readonly Mock<SampleMongoDb> _db;
    private readonly DeleteIntIdEntityHandler _sut;

    public DeleteIntIdEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new(1);
    }

    [Fact]
    public async Task Should_DoNothingWhenEntityDoesNotExist() {
        // Arrange
        _db.Setup(x => x.FindAsync<IntIdEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IntIdEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new());

        // Assert
        await act.Should().NotThrowAsync();
        _db.Verify(
            x => x.FindAsync<IntIdEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Should_RemoveFromDbSetAndSave() {
        // Arrange
        _db.Setup(x => x.FindAsync<IntIdEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IntIdEntity { Id = _command.Id, Name = "Test entity" });

        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(x => x.Remove(It.IsAny<IntIdEntity>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<IntIdEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }
}