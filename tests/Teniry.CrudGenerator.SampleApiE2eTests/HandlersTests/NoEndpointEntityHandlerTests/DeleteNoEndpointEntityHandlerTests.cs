using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.NoEndpointEntityFeature.DeleteNoEndpointEntity;
using Moq;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.NoEndpointEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.NoEndpointEntityHandlerTests;

public class DeleteNoEndpointEntityHandlerTests {
    private readonly DeleteNoEndpointEntityCommand _command;
    private readonly Mock<SampleMongoDb> _db;
    private readonly DeleteNoEndpointEntityHandler _sut;

    public DeleteNoEndpointEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid());
    }

    [Fact]
    public async Task Should_DoNothingWhenEntityDoesNotExist() {
        // Arrange
        _db.Setup(x => x.FindAsync<NoEndpointEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NoEndpointEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new());

        // Assert
        await act.Should().NotThrowAsync();
        _db.Verify(
            x => x.FindAsync<NoEndpointEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Should_RemoveFromDbSetAndSave() {
        // Arrange
        _db.Setup(x => x.FindAsync<NoEndpointEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NoEndpointEntity { Id = _command.Id, Name = "Test entity" });

        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(x => x.Remove(It.IsAny<NoEndpointEntity>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<NoEndpointEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }
}