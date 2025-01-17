using ITech.Cqrs.Domain.Exceptions;
using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.IntIdEntityFeature.UpdateIntIdEntity;
using Teniry.CrudGenerator.SampleApi.Generators.IntIdEntityGenerator;
using Moq;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.IntIdEntityHandlerTests;

public class UpdateIntIdEntityHandlerTests {
    private readonly UpdateIntIdEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly UpdateIntIdEntityHandler _sut;

    public UpdateIntIdEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new(1) {
            Name = "New entity name"
        };
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_UpdatingNotExistingEntity() {
        // Arrange
        _db.Setup(x => x.FindAsync<IntIdEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IntIdEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new());

        // Assert
        await act.Should().ThrowAsync<EfEntityNotFoundException>()
            .Where(x => x.TypeName.Equals(nameof(IntIdEntity)));
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave() {
        // Arrange
        var entity = new IntIdEntity { Id = _command.Id, Name = "Old entity name" };
        _db.Setup(x => x.FindAsync<IntIdEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        await _sut.HandleAsync(_command, new());

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