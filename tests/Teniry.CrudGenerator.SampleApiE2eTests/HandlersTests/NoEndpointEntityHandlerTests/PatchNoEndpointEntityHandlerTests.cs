using Teniry.Cqrs.Extended.Exceptions;
using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.NoEndpointEntityFeature.UpdateNoEndpointEntity;
using Moq;
using Teniry.Cqrs.Extended.Types.PatchOperationType;
using Teniry.CrudGenerator.SampleApi.Application.NoEndpointEntityFeature.PatchNoEndpointEntity;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.NoEndpointEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.NoEndpointEntityHandlerTests;

public class PatchNoEndpointEntityHandlerTests {
    private readonly PatchNoEndpointEntityCommand _command;
    private readonly Mock<SampleMongoDb> _db;
    private readonly PatchNoEndpointEntityHandler _sut;

    public PatchNoEndpointEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid()) {
            Name = new("New entity name", PatchOpType.Update)
        };
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_UpdatingNotExistingEntity() {
        // Arrange
        _db.Setup(x => x.FindAsync<NoEndpointEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NoEndpointEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>()
            .Where(x => x.NotFoundType == typeof(NoEndpointEntity));
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave() {
        // Arrange
        var entity = new NoEndpointEntity { Id = _command.Id, Name = "Old entity name" };
        _db.Setup(x => x.FindAsync<NoEndpointEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        await _sut.HandleAsync(_command, CancellationToken.None);

        // Assert
        entity.Name.Should().Be("New entity name");
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<NoEndpointEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }
}