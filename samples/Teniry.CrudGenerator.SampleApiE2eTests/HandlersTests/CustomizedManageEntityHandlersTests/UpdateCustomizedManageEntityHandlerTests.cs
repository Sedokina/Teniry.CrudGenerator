using ITech.Cqrs.Domain.Exceptions;
using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.CustomManagedEntityFeature.ManagedEntityUpdateOperationCustomNs;
using Teniry.CrudGenerator.SampleApi.Generators.CustomManagedEntityGenerator;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;
using Moq;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.CustomizedManageEntityHandlersTests;

public class UpdateCustomizedManageEntityHandlerTests {
    private readonly CustomizedNameUpdateManagedEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly CustomizedNameUpdateManagedEntityHandler _sut;

    public UpdateCustomizedManageEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid()) {
            Name = "New entity name"
        };
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_UpdatingNotExistingEntity() {
        // Arrange
        _db.Setup(x => x.FindAsync<CustomManagedEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomManagedEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new());

        // Assert
        await act.Should().ThrowAsync<EfEntityNotFoundException>()
            .Where(x => x.TypeName.Equals(nameof(CustomManagedEntity)));
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave() {
        // Arrange
        var entity = new CustomManagedEntity { Id = _command.Id, Name = "Old entity name" };
        _db.Setup(x => x.FindAsync<CustomManagedEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        entity.Name.Should().Be("New entity name");
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<CustomManagedEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
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