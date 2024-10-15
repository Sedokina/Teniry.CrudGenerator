using ITech.Cqrs.Domain.Exceptions;
using ITech.CrudGenerator.Tests.Application.CompanyFeature.UpdateCompany;
using Moq;

namespace ITech.CrudGenerator.Tests;

public class UpdateHandlerTests
{
    private readonly Mock<TestMongoDb> _db;
    private readonly UpdateCompanyHandler _sut;
    private readonly UpdateCompanyCommand _command;

    public UpdateHandlerTests()
    {
        _db = new();
        _sut = new(_db.Object);
        _command = new UpdateCompanyCommand(Guid.NewGuid())
        {
            Name = "New company name"
        };
    }

    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_UpdatingNotExistingEntity()
    {
        // Arrange
        _db.Setup(x => x.FindAsync<Company>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Company?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        await act.Should().ThrowAsync<EfEntityNotFoundException>()
            .Where(x => x.TypeName.Equals(nameof(Company)));
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave()
    {
        // Arrange
        var company = new Company { Id = _command.Id, Name = "Old company name" };
        _db.Setup(x => x.FindAsync<Company>(new object[] { _command.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);

        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        company.Name.Should().Be("New company name");
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.Verify(x => x.FindAsync<Company>(new object[] { _command.Id }, It.IsAny<CancellationToken>()), Times.Once);
        _db.VerifyNoOtherCalls();
    }
}