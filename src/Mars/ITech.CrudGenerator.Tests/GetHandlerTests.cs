using ITech.Cqrs.Domain.Exceptions;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CompanyFeature.GetCompany;
using ITech.CrudGenerator.TestApi.Generators.CompanyGenerator;
using Moq;

namespace ITech.CrudGenerator.Tests;

public class GetHandlerTests
{
    private readonly Mock<TestMongoDb> _db;
    private readonly GetCompanyHandler _sut;
    private readonly GetCompanyQuery _query;

    public GetHandlerTests()
    {
        _db = new();
        _sut = new(_db.Object);
        _query = new GetCompanyQuery(Guid.NewGuid());
    }


    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_GettingNotExistingEntity()
    {
        // Arrange
        _db.Setup(x => x.FindAsync<Company>(new object[] { _query.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Company?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_query, new CancellationToken());

        // Assert
        await act.Should().ThrowAsync<EfEntityNotFoundException>()
            .Where(x => x.TypeName.Equals(nameof(Company)));
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave()
    {
        // Arrange
        _db.Setup(x => x.FindAsync<Company>(new object[] { _query.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Company { Id = _query.Id, Name = "My test company" });

        // Act
        var company = await _sut.HandleAsync(_query, new CancellationToken());

        // Assert
        company.Id.Should().Be(_query.Id);
        company.Name.Should().Be("My test company");
        _db.Verify(x => x.FindAsync<Company>(new object[] { _query.Id }, It.IsAny<CancellationToken>()), Times.Once);
        _db.VerifyNoOtherCalls();
    }
}