using ITech.Cqrs.Domain.Exceptions;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.NoEndpointEntityFeature.UpdateNoEndpointEntity;
using ITech.CrudGenerator.TestApi.Generators.NoEndpointEntityGenerator;
using Moq;

namespace ITech.CrudGenerator.TestApiTests.HandlersTests.NoEndpointEntityHandlerTests;

public class UpdateNoEndpointEntityHandlerTests
{
    private readonly UpdateNoEndpointEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly UpdateNoEndpointEntityHandler _sut;

    public UpdateNoEndpointEntityHandlerTests()
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
        _db.Setup(x => x.FindAsync<NoEndpointEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NoEndpointEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        await act.Should().ThrowAsync<EfEntityNotFoundException>()
            .Where(x => x.TypeName.Equals(nameof(NoEndpointEntity)));
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave()
    {
        // Arrange
        var entity = new NoEndpointEntity { Id = _command.Id, Name = "Old entity name" };
        _db.Setup(x => x.FindAsync<NoEndpointEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        entity.Name.Should().Be("New entity name");
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(x => x.FindAsync<NoEndpointEntity>(new object[] { _command.Id }, It.IsAny<CancellationToken>()),
            Times.Once);
        _db.VerifyNoOtherCalls();
    }
}