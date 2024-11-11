using ITech.Cqrs.Domain.Exceptions;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.EntityIdNameFeature.UpdateEntityIdName;
using ITech.CrudGenerator.TestApi.Generators.CustomIds.EntityIdNameGenerator;
using Moq;

namespace ITech.CrudGenerator.TestApiTests.HandlersTests.EntityIdNameGeneratorHandlerTests;

public class UpdateEntityIdNameHandlerTests
{
    private readonly UpdateEntityIdNameCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly UpdateEntityIdNameHandler _sut;

    public UpdateEntityIdNameHandlerTests()
    {
        _db = new Mock<TestMongoDb>();
        _sut = new(_db.Object);
        _command = new(Guid.NewGuid())
        {
            Name = "New entity name"
        };
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_UpdatingNotExistingEntity()
    {
        // Arrange
        _db.Setup(x =>
                x.FindAsync<EntityIdName>(new object[] { _command.EntityIdNameId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((EntityIdName?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        await act.Should().ThrowAsync<EfEntityNotFoundException>()
            .Where(x => x.TypeName.Equals(nameof(EntityIdName)));
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave()
    {
        // Arrange
        var entity = new EntityIdName { EntityIdNameId = _command.EntityIdNameId, Name = "Old entity name" };
        _db.Setup(x =>
                x.FindAsync<EntityIdName>(new object[] { _command.EntityIdNameId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        entity.Name.Should().Be("New entity name");
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(
            x => x.FindAsync<EntityIdName>(new object[] { _command.EntityIdNameId }, It.IsAny<CancellationToken>()),
            Times.Once);
        _db.VerifyNoOtherCalls();
    }
}