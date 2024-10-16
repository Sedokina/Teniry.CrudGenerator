using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CompanyFeature.DeleteCompany;
using ITech.CrudGenerator.TestApi.Generators.CompanyGenerator;
using Moq;

namespace ITech.CrudGenerator.Tests.HandlersTests;

public class DeleteHandlerTests
{
    private readonly DeleteCompanyCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly DeleteCompanyHandler _sut;

    public DeleteHandlerTests()
    {
        _db = new Mock<TestMongoDb>();
        _sut = new DeleteCompanyHandler(_db.Object);
        _command = new DeleteCompanyCommand(Guid.NewGuid());
    }

    [Fact]
    public async Task Should_DoNothingWhenEntityDoesNotExist()
    {
        // Arrange
        _db.Setup(x => x.FindAsync<Company>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Company?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        await act.Should().NotThrowAsync();
        _db.Verify(x => x.FindAsync<Company>(new object[] { _command.Id }, It.IsAny<CancellationToken>()), Times.Once);
        _db.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Should_RemoveFromDbSetAndSave()
    {
        // Arrange
        _db.Setup(x => x.FindAsync<Company>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Company { Id = _command.Id, Name = "Test company" });

        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        _db.Verify(x => x.Remove(It.IsAny<Company>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(x => x.FindAsync<Company>(new object[] { _command.Id }, It.IsAny<CancellationToken>()), Times.Once);
        _db.VerifyNoOtherCalls();
    }
}