using ITech.Cqrs.Domain.Exceptions;
using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.SimpleEntityFeature.UpdateSimpleEntity;
using Teniry.CrudGenerator.SampleApi.Generators.SimpleEntityGenerator;
using Moq;

namespace Teniry.CrudGenerator.TestApiTests.HandlersTests.SimpleEntityHandlersTests;

public class UpdateSimpleEntityHandlerTests {
    private readonly UpdateSimpleEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly UpdateSimpleEntityHandler _sut;

    public UpdateSimpleEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid()) {
            Name = "New entity name"
        };
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_UpdatingNotExistingEntity() {
        // Arrange
        _db.Setup(x => x.FindAsync<SimpleEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SimpleEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new());

        // Assert
        await act.Should().ThrowAsync<EfEntityNotFoundException>()
            .Where(x => x.TypeName.Equals(nameof(SimpleEntity)));
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave() {
        // Arrange
        var entity = new SimpleEntity { Id = _command.Id, Name = "Old entity name" };
        _db.Setup(x => x.FindAsync<SimpleEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        entity.Name.Should().Be("New entity name");
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<SimpleEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }
}