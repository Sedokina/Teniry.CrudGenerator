using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.CustomManagedEntityFeature.ManagedEntityDeleteOperationCustomNs;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;
using Moq;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomManagedEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.CustomizedManageEntityHandlersTests;

public class DeleteCustomManagedEntityHandlerTests {
    private readonly CustomizedNameDeleteManagedEntityCommand _command;
    private readonly Mock<SampleMongoDb> _db;
    private readonly CustomizedNameDeleteManagedEntityHandler _sut;

    public DeleteCustomManagedEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid());
    }

    [Fact]
    public async Task Should_DoNothingWhenEntityDoesNotExist() {
        // Arrange
        _db.Setup(x => x.FindAsync<CustomManagedEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomManagedEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new());

        // Assert
        await act.Should().NotThrowAsync();
        _db.Verify(
            x => x.FindAsync<CustomManagedEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Should_RemoveFromDbSetAndSave() {
        // Arrange
        _db.Setup(x => x.FindAsync<CustomManagedEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CustomManagedEntity { Id = _command.Id, Name = "Test entity" });

        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(x => x.Remove(It.IsAny<CustomManagedEntity>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<CustomManagedEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _db.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("CustomizedNameDeleteManagedEntityCommand")]
    [InlineData("CustomizedNameDeleteManagedEntityHandler")]
    public void Should_BeInCustomNamespace(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().BeInNamespaceThatEndsWith(typeName, "ManagedEntityDeleteOperationCustomNs");
    }
}