using Moq;
using Teniry.Cqrs.Extended.Exceptions;
using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.WriteOnlyCustomizedEntityFeature.ManagedEntityUpdateOperationCustomNs;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.WriteOnlyCustomizedGenerator;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.WriteOnlyCustomizedEntityHandlersTests;

public class UpdateWriteOnlyCustomizedEntityHandlerTests {
    private readonly CustomizedNameUpdateManagedEntityCommand _command;
    private readonly Mock<SampleMongoDb> _db;
    private readonly CustomizedNameUpdateManagedEntityHandler _sut;

    public UpdateWriteOnlyCustomizedEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid()) {
            Name = "New entity name"
        };
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_UpdatingNotExistingEntity() {
        // Arrange
        _db.Setup(x => x.FindAsync<WriteOnlyCustomizedEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WriteOnlyCustomizedEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new());

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>()
            .Where(x => x.NotFoundType == typeof(WriteOnlyCustomizedEntity));
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave() {
        // Arrange
        var entity = new WriteOnlyCustomizedEntity { Id = _command.Id, Name = "Old entity name" };
        _db.Setup(x => x.FindAsync<WriteOnlyCustomizedEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        entity.Name.Should().Be("New entity name");
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<WriteOnlyCustomizedEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("CustomizedNameUpdateManagedEntityCommand")]
    [InlineData("CustomizedNameUpdateManagedEntityHandler")]
    public void Should_BeInCustomNamespace(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().BeInNamespaceThatEndsWith(typeName, "ManagedEntityUpdateOperationCustomNs");
    }
}